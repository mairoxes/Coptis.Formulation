namespace Coptis.Formulation.Application.Models;

public record RawMaterialListItem(
    string Id,
    string Name,
    decimal PriceAmount,
    string Currency,
    string ReferenceUnit);