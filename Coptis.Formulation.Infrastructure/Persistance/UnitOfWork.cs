using System;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coptis.Formulation.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public Task<int> SaveChanges(CancellationToken ct) => _db.SaveChangesAsync(ct);

        public async Task<IAsyncDisposable> BeginTransaction(CancellationToken ct)
        {
            IDbContextTransaction tx = await _db.Database.BeginTransactionAsync(ct);
            return tx;
        }
    }
}
