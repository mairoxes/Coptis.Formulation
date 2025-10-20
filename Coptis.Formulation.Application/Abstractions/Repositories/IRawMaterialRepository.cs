using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Domain.Entities;

namespace Coptis.Formulation.Application.Abstractions.Repositories
{
    public interface IRawMaterialRepository
    {
        Task<RawMaterial?> FindByName(string name, CancellationToken ct);
        Task Add(RawMaterial rawMaterial, CancellationToken ct);
    }
}
