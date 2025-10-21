
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Coptis.Formulation.WebAssembly.Services
{
    public sealed class SubstancesApiClient
    {
        private readonly HttpClient _http;

        public SubstancesApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<IReadOnlyList<SubstanceUsageItem>> GetByWeight(CancellationToken ct = default)
        {
            try
            {
                var list = await _http.GetFromJsonAsync<List<SubstanceUsageItem>>("api/Substances/by-weight", ct);
                return list ?? new List<SubstanceUsageItem>();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error fetching substances by weight: {ex.Message}");
                return new List<SubstanceUsageItem>();
            }
        }

        public async Task<IReadOnlyList<SubstanceUsageItem>> GetByCount(CancellationToken ct = default)
        {
            try
            {
                var list = await _http.GetFromJsonAsync<List<SubstanceUsageItem>>("api/Substances/by-count", ct);
                return list ?? new List<SubstanceUsageItem>();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error fetching substances by count: {ex.Message}");
                return new List<SubstanceUsageItem>();
            }
        }

        public record SubstanceUsageItem(
            string Name,
            decimal TotalWeight,
            int FormulaCount);
    }
}