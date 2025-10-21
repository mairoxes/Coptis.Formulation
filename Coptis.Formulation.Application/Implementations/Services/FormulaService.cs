using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Implementations.Services
{
    public sealed class FormulaService : IFormulaService
    {
        private readonly IFormulaRepository _repo;
        private readonly IUnitOfWork _uow;

        public FormulaService(IFormulaRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<IReadOnlyList<FormulaListItem>> GetAll(string? query, CancellationToken ct)
        {
            var all = await _repo.GetAll(ct);

            if (!string.IsNullOrWhiteSpace(query))
                all = all.Where(f => f.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            return all
                .OrderBy(f => f.Name)
                .Select(f => new FormulaListItem(
                    f.Id.ToString(),
                    f.Name,
                    decimal.Round(f.BatchWeight, 2),
                    f.WeightUnit,
                    decimal.Round(f.TotalCost, 2),
                    f.IsHighlighted))
                .ToList();
        }

        public async Task<bool> Delete(Guid id, CancellationToken ct)
        {
            var formula = await _repo.FindById(id, ct);

            if (formula == null)
                return false;

            await _repo.Delete(formula, ct);
            await _uow.SaveChanges(ct);

            return true;
        }
    }
}