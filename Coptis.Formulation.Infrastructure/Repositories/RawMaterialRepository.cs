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
    public sealed class RawMaterialRepository : IRawMaterialRepository
    {
        private readonly AppDbContext _db;

        public RawMaterialRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<RawMaterial?> FindByName(string name, CancellationToken ct) =>
            _db.RawMaterials.FirstOrDefaultAsync(r => r.Name == name, ct);

        public Task<RawMaterial?> FindByNameWithSubstances(string name, CancellationToken ct) =>
            _db.RawMaterials
                .Include(r => r.SubstanceShares)
                .FirstOrDefaultAsync(r => r.Name == name, ct);

        public Task<RawMaterial?> FindById(Guid id, CancellationToken ct) =>
            _db.RawMaterials.FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task Add(RawMaterial rawMaterial, CancellationToken ct) =>
            await _db.RawMaterials.AddAsync(rawMaterial, ct);

        public Task<List<RawMaterial>> GetAll(CancellationToken ct) =>
            _db.RawMaterials
              .AsNoTracking()
              .ToListAsync(ct);

        public async Task RemoveSubstanceShares(Guid rawMaterialId, CancellationToken ct)
        {
            var existingShares = await _db.RawMaterialSubstances
                .Where(rms => rms.RawMaterialId == rawMaterialId)
                .ToListAsync(ct);

            _db.RawMaterialSubstances.RemoveRange(existingShares);
        }
    }
}