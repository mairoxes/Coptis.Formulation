namespace Coptis.Formulation.Application.Models;

public enum ImportStatus { Success, SuccessWithIssues, Failed }

public record ImportIssue(string Code, string Message);

public record ImportResult(ImportStatus Status, List<ImportIssue> Issues)
{
    public static ImportResult Ok() => new(ImportStatus.Success, new());
    public static ImportResult OkWithIssues(List<ImportIssue> issues) => new(ImportStatus.SuccessWithIssues, issues);
    public static ImportResult Fail(List<ImportIssue> issues) => new(ImportStatus.Failed, issues);
}
