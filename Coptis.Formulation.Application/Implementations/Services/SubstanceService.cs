using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Implementations.Services
{
    public sealed class SubstanceService : ISubstanceService
    {
        private readonly ISubstanceRepository _substanceRepo;
        private readonly IFormulaRepository _formulaRepo;

        public SubstanceService(
            ISubstanceRepository substanceRepo,
            IFormulaRepository formulaRepo)
        {
            _substanceRepo = substanceRepo;
            _formulaRepo = formulaRepo;
        }

        public async Task<IReadOnlyList<SubstanceUsageItem>> GetMostUsedByWeight(CancellationToken ct)
        {
            var formulas = await _formulaRepo.GetAllWithDetails(ct);

            var substanceWeights = new Dictionary<string, decimal>();

            foreach (var formula in formulas)
            {
                foreach (var component in formula.Components)
                {
                    var rawMaterial = component.RawMaterial;
                    var componentWeightInFormula = component.EffectiveWeight;

                    foreach (var substanceShare in rawMaterial.SubstanceShares)
                    {
                        var substanceName = substanceShare.Substance.Name;
                        var substanceWeight = componentWeightInFormula * (substanceShare.Percentage / 100m);

                        if (substanceWeights.ContainsKey(substanceName))
                            substanceWeights[substanceName] += substanceWeight;
                        else
                            substanceWeights[substanceName] = substanceWeight;
                    }
                }
            }

            return substanceWeights
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new SubstanceUsageItem(
                    kv.Key,
                    decimal.Round(kv.Value, 2),
                    0)) 
                .ToList();
        }

        public async Task<IReadOnlyList<SubstanceUsageItem>> GetMostUsedByFormulaCount(CancellationToken ct)
        {
            var formulas = await _formulaRepo.GetAllWithDetails(ct);

            var substanceFormulaCounts = new Dictionary<string, HashSet<string>>();

            foreach (var formula in formulas)
            {
                foreach (var component in formula.Components)
                {
                    var rawMaterial = component.RawMaterial;

                    foreach (var substanceShare in rawMaterial.SubstanceShares)
                    {
                        var substanceName = substanceShare.Substance.Name;

                        if (!substanceFormulaCounts.ContainsKey(substanceName))
                            substanceFormulaCounts[substanceName] = new HashSet<string>();

                        substanceFormulaCounts[substanceName].Add(formula.Name);
                    }
                }
            }

            return substanceFormulaCounts
                .OrderByDescending(kv => kv.Value.Count)
                .Select(kv => new SubstanceUsageItem(
                    kv.Key,
                    0, // TotalWeight not needed for this view
                    kv.Value.Count))
                .ToList();
        }
    }
}