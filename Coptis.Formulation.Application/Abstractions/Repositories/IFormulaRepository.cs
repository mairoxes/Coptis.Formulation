using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Domain.Entities;

namespace Coptis.Formulation.Application.Abstractions.Repositories;

public interface IFormulaRepository
{
    Task<Formula?> FindByName(string name, CancellationToken ct);
    Task Add(Formula formula, CancellationToken ct);
    Task Delete(Formula formula, CancellationToken ct);
    Task<List<Formula>> GetAll(CancellationToken ct);   // ← add this
}
