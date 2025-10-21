namespace Coptis.Formulation.Application.Models;

public record UpdatePriceResult(
    bool Success,
    string? ErrorMessage,
    int AffectedFormulasCount)
{
    public static UpdatePriceResult Ok(int affectedCount) =>
        new(true, null, affectedCount);

    public static UpdatePriceResult Fail(string error) =>
        new(false, error, 0);
}