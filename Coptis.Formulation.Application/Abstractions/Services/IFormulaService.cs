using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Abstractions.Services;

public interface IFormulaService
{
    Task<IReadOnlyList<FormulaListItem>> GetAll(string? query, CancellationToken ct);
}
