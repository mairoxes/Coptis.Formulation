using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Domain.Entities;

namespace Coptis.Formulation.Application.Abstractions.Repositories
{
    public interface ISubstanceRepository
    {
        Task<Substance?> FindByName(string name, CancellationToken ct);
        Task Add(Substance substance, CancellationToken ct);
    }
}
