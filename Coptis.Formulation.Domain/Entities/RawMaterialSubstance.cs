namespace Coptis.Formulation.Domain.Entities;

public class RawMaterialSubstance
{
    public Guid RawMaterialId { get; set; }
    public Guid SubstanceId { get; set; }
    public decimal Percentage { get; set; }
    public RawMaterial RawMaterial { get; set; } = default!;
    public Substance Substance { get; set; } = default!;
}
