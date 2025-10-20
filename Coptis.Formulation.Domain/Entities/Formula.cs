namespace Coptis.Formulation.Domain.Entities;

public class Formula
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal BatchWeight { get; set; }
    public string WeightUnit { get; set; } = "g";
    public decimal TotalCost { get; set; }
    public bool IsHighlighted { get; set; }
    public ICollection<FormulaComponent> Components { get; set; } = new List<FormulaComponent>();
}
