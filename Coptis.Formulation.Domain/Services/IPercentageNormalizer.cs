namespace Coptis.Formulation.Domain.Services;

public interface IPercentageNormalizer
{
    decimal[] NormalizeTo100(decimal[] values, decimal tolerance = 0.5m);
}
