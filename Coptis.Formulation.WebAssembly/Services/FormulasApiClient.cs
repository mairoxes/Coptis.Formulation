using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Contracts.Import.Dtos;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.WebAssembly.Services;

public sealed class FormulasApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<FormulaListItem>> GetAll(string? q = null, CancellationToken ct = default)
    {
        var url = string.IsNullOrWhiteSpace(q) ? "api/formulas" : $"api/formulas?q={Uri.EscapeDataString(q)}";
        var list = await http.GetFromJsonAsync<List<FormulaListItem>>(url, ct);
        return list ?? new List<FormulaListItem>();
    }
    public async Task<ImportResult> Import(FormulaDto dto, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/formulas/import", dto, ct);
        var result = await response.Content.ReadFromJsonAsync<ImportResult>(cancellationToken: ct);

        return response.IsSuccessStatusCode
            ? result ?? ImportResult.Ok()
            : result ?? ImportResult.Fail(new());
    }
}
