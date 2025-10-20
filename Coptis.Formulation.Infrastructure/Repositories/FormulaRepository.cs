using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coptis.Formulation.Infrastructure.Persistence.Repositories;

public sealed class FormulaRepository(AppDbContext db) : IFormulaRepository
{
    public Task<Formula?> FindByName(string name, CancellationToken ct) =>
        db.Formulas
          .Include(f => f.Components)
          .ThenInclude(c => c.RawMaterial)
          .FirstOrDefaultAsync(f => f.Name == name, ct);

    public async Task Add(Formula formula, CancellationToken ct) =>
        await db.Formulas.AddAsync(formula, ct);

    public Task Delete(Formula formula, CancellationToken ct)
    {
        db.Formulas.Remove(formula);
        return Task.CompletedTask;
    }

    public Task<List<Formula>> GetAll(CancellationToken ct) =>
        db.Formulas
          .AsNoTracking()
          .ToListAsync(ct);
}
