namespace Coptis.Formulation.Domain.Entities;

public class FormulaComponent
{
    public Guid FormulaId { get; set; }
    public Guid RawMaterialId { get; set; }
    public decimal Percentage { get; set; }
    public decimal EffectiveWeight { get; set; }
    public decimal CostShare { get; set; }
    public Formula Formula { get; set; } = default!;
    public RawMaterial RawMaterial { get; set; } = default!;
}
