using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Domain.Entities;
using Coptis.Formulation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Coptis.Formulation.Infrastructure.Repositories
{
    public sealed class SubstanceRepository : ISubstanceRepository
    {
        private readonly AppDbContext _db;

        public SubstanceRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<Substance?> FindByName(string name, CancellationToken ct)
        {
            return _db.Substances.FirstOrDefaultAsync(s => s.Name == name, ct);
        }

        public async Task Add(Substance substance, CancellationToken ct)
        {
            await _db.Substances.AddAsync(substance, ct);
        }
    }
}
