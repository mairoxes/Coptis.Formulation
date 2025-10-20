using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Contracts.Import.Dtos;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Implementations.Services
{
    public sealed class FormulaValidationService : IFormulaValidationService
    {
        public Task<(bool isValid, List<ImportIssue> issues)> Validate(FormulaDto dto, CancellationToken ct)
        {
            var issues = new List<ImportIssue>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                issues.Add(new ImportIssue("name_empty", "Name is required"));

            if (dto.Weight <= 0)
                issues.Add(new ImportIssue("weight_invalid", "Weight must be > 0"));

            if (string.IsNullOrWhiteSpace(dto.WeightUnit))
                issues.Add(new ImportIssue("weight_unit_missing", "Weight unit is required"));

            if (dto.RawMaterials == null || dto.RawMaterials.Count == 0)
                issues.Add(new ImportIssue("raw_materials_empty", "At least one raw material is required"));

            if (dto.RawMaterialPercentages == null || dto.RawMaterialPercentages.Count == 0)
                issues.Add(new ImportIssue("rm_percentages_empty", "Raw material percentages are required"));

            if (dto.RawMaterials != null && dto.RawMaterialPercentages != null)
            {
                if (dto.RawMaterials.Count != dto.RawMaterialPercentages.Count)
                    issues.Add(new ImportIssue("rm_counts_mismatch", "Raw materials count must match percentages count"));

                var total = dto.RawMaterialPercentages.Select(x => Math.Round(x, 2)).Sum();
                if (total < 99.99m || total > 100.01m)
                    issues.Add(new ImportIssue("rm_sum_invalid", "Raw material percentages must sum to 100% ±0.01, actual total here: "+ total));

                for (int i = 0; i < dto.RawMaterials.Count; i++)
                {
                    var rm = dto.RawMaterials[i];

                    if (string.IsNullOrWhiteSpace(rm.Name))
                        issues.Add(new ImportIssue("rm_name_empty", $"Raw material name is required at index {i}"));

                    if (rm.Price == null)
                        issues.Add(new ImportIssue("rm_price_missing", $"Price missing at index {i}"));
                    else
                    {
                        if (rm.Price.Amount <= 0)
                            issues.Add(new ImportIssue("rm_price_invalid", $"Price must be > 0 at index {i}"));
                        if (!string.Equals(rm.Price.Currency, "EUR"))
                            issues.Add(new ImportIssue("rm_currency_invalid", $"Currency must be EUR at index {i}"));
                        if (!string.Equals(rm.Price.ReferenceUnit, "kg"))
                            issues.Add(new ImportIssue("rm_unit_invalid", $"Reference unit must be kg at index {i}"));
                    }

                    if (rm.Substances == null || rm.Substances.Count == 0)
                        issues.Add(new ImportIssue("substances_empty", $"Substances missing at index {i}"));

                    if (rm.SubstancePercentages == null || rm.SubstancePercentages.Count == 0)
                        issues.Add(new ImportIssue("substance_percentages_empty", $"Substance percentages missing at index {i}"));

                    if (rm.Substances != null && rm.SubstancePercentages != null)
                    {
                        if (rm.Substances.Count != rm.SubstancePercentages.Count)
                            issues.Add(new ImportIssue("substance_count_mismatch", $"Substances count must match percentages at index {i}"));

                        var sum = rm.SubstancePercentages.Sum();
                        if (sum < 99.99m || sum > 100.01m)
                            issues.Add(new ImportIssue("substance_sum_invalid", $"Substance percentages must sum to 100% ±0.01 at index {i}"));
                    }
                }
            }

            var ok = issues.Count == 0;
            return Task.FromResult((ok, issues));
        }
    }
}
