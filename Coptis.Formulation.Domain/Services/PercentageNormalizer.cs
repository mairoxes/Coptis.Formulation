using System;
using System.Linq;

namespace Coptis.Formulation.Domain.Services;

public sealed class PercentageNormalizer : IPercentageNormalizer
{
    public decimal[] NormalizeTo100(decimal[] values, decimal tolerance = 0.5m)
    {
        var sum = values.Sum();
        if (sum <= 0) return values;
        if (Math.Abs(sum - 100m) <= tolerance) return values;
        return values.Select(v => Math.Round(v * 100m / sum, 4, MidpointRounding.AwayFromZero)).ToArray();
    }
}
