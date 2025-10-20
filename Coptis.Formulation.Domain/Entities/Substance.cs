namespace Coptis.Formulation.Domain.Entities;

public class Substance
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<RawMaterialSubstance> InRawMaterials { get; set; } = new List<RawMaterialSubstance>();
}
