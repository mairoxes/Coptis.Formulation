using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Contracts.Import.Dtos;
using Coptis.Formulation.Application.Models;
using Coptis.Formulation.Domain.Entities;

namespace Coptis.Formulation.Application.Implementations.Services
{
    public sealed class FormulaImportService : IFormulaImportService
    {
        private readonly IFormulaRepository _formulaRepo;
        private readonly IRawMaterialRepository _rawMaterialRepo;
        private readonly IUnitOfWork _uow;

        public FormulaImportService(
            IFormulaRepository formulaRepo,
            IRawMaterialRepository rawMaterialRepo,
            IUnitOfWork uow)
        {
            _formulaRepo = formulaRepo;
            _rawMaterialRepo = rawMaterialRepo;
            _uow = uow;
        }

        public async Task<ImportResult> Import(FormulaDto dto, CancellationToken ct)
        {
            var existing = await _formulaRepo.FindByName(dto.Name, ct);
            if (existing != null)
                await _formulaRepo.Delete(existing, ct);

            var formula = new Formula
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                BatchWeight = dto.Weight,
                WeightUnit = dto.WeightUnit,
                IsHighlighted = false
            };

            for (int i = 0; i < dto.RawMaterials.Count; i++)
            {
                var rmDto = dto.RawMaterials[i];
                var percentage = dto.RawMaterialPercentages.ElementAtOrDefault(i);

                var rawMaterial = await _rawMaterialRepo.FindByName(rmDto.Name, ct);
                if (rawMaterial == null)
                {
                    rawMaterial = new RawMaterial
                    {
                        Id = Guid.NewGuid(),
                        Name = rmDto.Name,
                        PriceAmount = rmDto.Price.Amount,
                        Currency = rmDto.Price.Currency,
                        ReferenceUnit = rmDto.Price.ReferenceUnit
                    };
                    await _rawMaterialRepo.Add(rawMaterial, ct);
                }
                else
                {
                    rawMaterial.PriceAmount = rmDto.Price.Amount;
                    rawMaterial.Currency = rmDto.Price.Currency;
                    rawMaterial.ReferenceUnit = rmDto.Price.ReferenceUnit;
                }

                var effectiveWeightG = Math.Round(dto.Weight * percentage / 100m, 2, MidpointRounding.AwayFromZero);
                var costShare = Math.Round((effectiveWeightG / 1000m) * rawMaterial.PriceAmount, 2, MidpointRounding.AwayFromZero);

                formula.Components.Add(new FormulaComponent
                {
                    FormulaId = formula.Id,
                    RawMaterialId = rawMaterial.Id,
                    Percentage = Math.Round(percentage, 2, MidpointRounding.AwayFromZero),
                    EffectiveWeight = effectiveWeightG,
                    CostShare = costShare
                });
            }

            formula.TotalCost = Math.Round(formula.Components.Sum(c => c.CostShare), 2, MidpointRounding.AwayFromZero);

            await _formulaRepo.Add(formula, ct);
            await _uow.SaveChanges(ct);

            return ImportResult.Ok();
        }
    }
}
