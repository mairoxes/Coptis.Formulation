using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Abstractions.Services
{
    public interface ISubstanceService
    {
        Task<IReadOnlyList<SubstanceUsageItem>> GetMostUsedByWeight(CancellationToken ct);
        Task<IReadOnlyList<SubstanceUsageItem>> GetMostUsedByFormulaCount(CancellationToken ct);
    }
}