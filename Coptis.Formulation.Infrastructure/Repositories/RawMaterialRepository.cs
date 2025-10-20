using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Domain.Entities;
using Coptis.Formulation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Coptis.Formulation.Infrastructure.Persistence.Repositories;

public class RawMaterialRepository(AppDbContext db) : IRawMaterialRepository
{
    public Task<RawMaterial?> FindByName(string name, CancellationToken ct) =>
        db.RawMaterials.FirstOrDefaultAsync(r => r.Name == name, ct);

    public async Task Add(RawMaterial rawMaterial, CancellationToken ct) =>
        await db.RawMaterials.AddAsync(rawMaterial, ct);
}
