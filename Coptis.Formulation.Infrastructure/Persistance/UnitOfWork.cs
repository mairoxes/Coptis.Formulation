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

        public async Task<int> SaveChanges(CancellationToken ct)
        {
            try
            {
                var result = await _db.SaveChangesAsync(ct);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IAsyncDisposable> BeginTransaction(CancellationToken ct)
        {
            IDbContextTransaction tx = await _db.Database.BeginTransactionAsync(ct);
            return tx;
        }
    }
}
