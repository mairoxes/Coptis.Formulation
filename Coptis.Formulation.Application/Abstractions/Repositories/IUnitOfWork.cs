using System.Threading;
using System.Threading.Tasks;

namespace Coptis.Formulation.Application.Abstractions.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChanges(CancellationToken ct);
    Task<IAsyncDisposable> BeginTransaction(CancellationToken ct);
}
