using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Domain.Entities;
using Coptis.Formulation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Coptis.Formulation.Infrastructure.Persistence.Repositories
{
    public sealed class FormulaRepository : IFormulaRepository
    {
        private readonly AppDbContext _db;

        public FormulaRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<Formula?> FindByName(string name, CancellationToken ct) =>
            _db.Formulas
              .Include(f => f.Components)
              .ThenInclude(c => c.RawMaterial)
              .FirstOrDefaultAsync(f => f.Name == name, ct);

        public Task<Formula?> FindById(Guid id, CancellationToken ct) =>
            _db.Formulas
              .Include(f => f.Components)
              .ThenInclude(c => c.RawMaterial)
              .FirstOrDefaultAsync(f => f.Id == id, ct);

        public async Task Add(Formula formula, CancellationToken ct) =>
            await _db.Formulas.AddAsync(formula, ct);

        public Task Delete(Formula formula, CancellationToken ct)
        {
            _db.Formulas.Remove(formula);
            return Task.CompletedTask;
        }

        public Task<List<Formula>> GetAll(CancellationToken ct) =>
            _db.Formulas
              .AsNoTracking()
              .ToListAsync(ct);

        public Task<List<Formula>> GetAllWithDetails(CancellationToken ct) =>
            _db.Formulas
              .AsNoTracking()
              .Include(f => f.Components)
                  .ThenInclude(c => c.RawMaterial)
                      .ThenInclude(rm => rm.SubstanceShares)
                          .ThenInclude(ss => ss.Substance)
              .ToListAsync(ct);

        public Task<List<Formula>> GetFormulasUsingRawMaterial(Guid rawMaterialId, CancellationToken ct) =>
            _db.Formulas
              .Include(f => f.Components)
                  .ThenInclude(c => c.RawMaterial)
              .Where(f => f.Components.Any(c => c.RawMaterialId == rawMaterialId))
              .ToListAsync(ct);
    }
}