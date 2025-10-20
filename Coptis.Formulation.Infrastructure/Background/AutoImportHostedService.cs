using System.Text;
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Contracts.Import.Dtos;
using Coptis.Formulation.Infrastructure.FileWatching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Coptis.Formulation.Infrastructure.Background;

public sealed class AutoImportHostedService(
    ILogger<AutoImportHostedService> logger,
    IServiceProvider services,
    IOptions<ImportOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var o = options.Value;
        if (!o.Enabled) return;
        EnsureDirectories(o);
        var jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        while (!ct.IsCancellationRequested)
        {
            try
            {
                foreach (var path in Directory.EnumerateFiles(o.WatchFolder, o.SearchPattern))
                {
                    if (ct.IsCancellationRequested) break;
                    await ProcessOne(path, o, jsonOpts, ct);
                }
            }
            catch { }

            try { await Task.Delay(TimeSpan.FromSeconds(o.PollSeconds), ct); }
            catch (TaskCanceledException) { }
        }
    }

    async Task ProcessOne(string path, ImportOptions o, JsonSerializerOptions jsonOpts, CancellationToken ct)
    {
        if (!IsFileReady(path)) return;

        FormulaDto? dto = null;
        try
        {
            var json = await File.ReadAllTextAsync(path, ct);
            dto = JsonSerializer.Deserialize<FormulaDto>(json, jsonOpts);
            if (dto is null)
            {
                await MoveToError(path, o, "Could not deserialize JSON", ct);
                return;
            }

            using var scope = services.CreateScope();
            var validator = scope.ServiceProvider.GetRequiredService<IFormulaValidationService>();
            var importer = scope.ServiceProvider.GetRequiredService<IFormulaImportService>();

            var (ok, issues) = await validator.Validate(dto, ct);
            if (!ok)
            {
                var reason = "Validation failed:\n" + string.Join("\n", issues.Select(i => $"{i.Code}: {i.Message}"));
                await MoveToError(path, o, reason, ct);
                return;
            }

            var result = await importer.Import(dto, ct);
            if (result.Status == Application.Models.ImportStatus.Failed)
            {
                var reason = "Import failed:\n" + string.Join("\n", result.Issues.Select(i => $"{i.Code}: {i.Message}"));
                await MoveToError(path, o, reason, ct);
                return;
            }

            await MoveToSuccess(path, o, ct);
        }
        catch (Exception ex)
        {
            await MoveToError(path, o, ex.ToString(), ct);
        }
    }

    static bool IsFileReady(string path)
    {
        try
        {
            using var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return s.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    static void EnsureDirectories(ImportOptions o)
    {
        Directory.CreateDirectory(o.WatchFolder);
        Directory.CreateDirectory(o.SuccessFolder);
        Directory.CreateDirectory(o.ErrorFolder);
    }

    static async Task MoveToSuccess(string src, ImportOptions o, CancellationToken ct)
    {
        var dst = Path.Combine(o.SuccessFolder, Stamp(Path.GetFileName(src)));
        await MoveFile(src, dst, ct);
    }

    static async Task MoveToError(string src, ImportOptions o, string reason, CancellationToken ct)
    {
        var name = Path.GetFileName(src);
        var dstJson = Path.Combine(o.ErrorFolder, Stamp(name));
        var dstTxt = Path.ChangeExtension(dstJson, ".txt");
        await MoveFile(src, dstJson, ct);
        await File.WriteAllTextAsync(dstTxt, reason, Encoding.UTF8, ct);
    }

    static async Task MoveFile(string src, string dst, CancellationToken ct)
    {
        for (var i = 0; i < 10; i++)
        {
            try
            {
                File.Move(src, dst, overwrite: true);
                return;
            }
            catch
            {
                await Task.Delay(150, ct);
            }
        }
        File.Move(src, dst, overwrite: true);
    }

    static string Stamp(string name)
    {
        var stamp = DateTimeOffset.Now.ToString("yyyyMMdd_HHmmss");
        var baseName = Path.GetFileNameWithoutExtension(name);
        var ext = Path.GetExtension(name);
        return $"{baseName}_{stamp}{ext}";
    }
}
