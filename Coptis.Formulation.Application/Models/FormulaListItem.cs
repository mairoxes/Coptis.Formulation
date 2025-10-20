namespace Coptis.Formulation.Application.Models;

public record FormulaListItem(string Id, string Name, decimal BatchWeight, string WeightUnit, decimal TotalCost, bool IsHighlighted);
