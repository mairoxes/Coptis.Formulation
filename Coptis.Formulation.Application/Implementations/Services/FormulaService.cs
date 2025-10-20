using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Implementations.Services;

public sealed class FormulaService(IFormulaRepository repo) : IFormulaService
{
    public async Task<IReadOnlyList<FormulaListItem>> GetAll(string? query, CancellationToken ct)
    {
        var all = await repo.GetAll(ct);
        if (!string.IsNullOrWhiteSpace(query))
            all = all.Where(f => f.Name.Contains(query)).ToList();

        return all
            .OrderBy(f => f.Name)
            .Select(f => new FormulaListItem(
                f.Id.ToString(),
                f.Name,
                decimal.Round(f.BatchWeight, 2),
                f.WeightUnit,
                decimal.Round(f.TotalCost, 2),
                f.IsHighlighted))
            .ToList();
    }
}
