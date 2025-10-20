namespace Coptis.Formulation.Domain.Entities;

public class RawMaterial
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal PriceAmount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string ReferenceUnit { get; set; } = "kg";
    public ICollection<FormulaComponent> UsedInComponents { get; set; } = new List<FormulaComponent>();
    public ICollection<RawMaterialSubstance> SubstanceShares { get; set; } = new List<RawMaterialSubstance>();
}
