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
    public sealed class RawMaterialService : IRawMaterialService
    {
        private readonly IRawMaterialRepository _rawMaterialRepo;
        private readonly IFormulaRepository _formulaRepo;
        private readonly IUnitOfWork _uow;

        public RawMaterialService(
            IRawMaterialRepository rawMaterialRepo,
            IFormulaRepository formulaRepo,
            IUnitOfWork uow)
        {
            _rawMaterialRepo = rawMaterialRepo;
            _formulaRepo = formulaRepo;
            _uow = uow;
        }

        public async Task<IReadOnlyList<RawMaterialListItem>> GetAll(CancellationToken ct)
        {
            var materials = await _rawMaterialRepo.GetAll(ct);

            return materials
                .OrderBy(m => m.Name)
                .Select(m => new RawMaterialListItem(
                    m.Id.ToString(),
                    m.Name,
                    decimal.Round(m.PriceAmount, 2),
                    m.Currency,
                    m.ReferenceUnit))
                .ToList();
        }

        public async Task<UpdatePriceResult> UpdatePrice(string id, decimal newPrice, CancellationToken ct)
        {
            if (!Guid.TryParse(id, out var guid))
                return UpdatePriceResult.Fail("Invalid ID format");

            var rawMaterial = await _rawMaterialRepo.FindById(guid, ct);

            if (rawMaterial == null)
                return UpdatePriceResult.Fail("Raw material not found");

            rawMaterial.PriceAmount = newPrice;

            var affectedFormulas = await _formulaRepo.GetFormulasUsingRawMaterial(guid, ct);

            foreach (var formula in affectedFormulas)
            {
                foreach (var component in formula.Components.Where(c => c.RawMaterialId == guid))
                {
                    var effectiveWeightKg = component.EffectiveWeight / 1000m;
                    component.CostShare = Math.Round(effectiveWeightKg * newPrice, 2, MidpointRounding.AwayFromZero);
                }

                formula.TotalCost = Math.Round(formula.Components.Sum(c => c.CostShare), 2, MidpointRounding.AwayFromZero);

                formula.IsHighlighted = true;
            }

            await _uow.SaveChanges(ct);

            return UpdatePriceResult.Ok(affectedFormulas.Count);
        }
    }
}