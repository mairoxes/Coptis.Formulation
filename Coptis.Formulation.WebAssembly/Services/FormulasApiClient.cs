using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Contracts.Import.Dtos;

namespace Coptis.Formulation.WebAssembly.Services
{
    public sealed class FormulasApiClient
    {
        private readonly HttpClient _http;

        public FormulasApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<IReadOnlyList<FormulaListItem>> GetAll(string? query = null, CancellationToken ct = default)
        {
            var url = string.IsNullOrWhiteSpace(query)
                ? "api/formulas"
                : $"api/formulas?q={Uri.EscapeDataString(query)}";

            var list = await _http.GetFromJsonAsync<List<FormulaListItem>>(url, ct);
            return list ?? new List<FormulaListItem>();
        }

        public async Task<ImportResult> Import(FormulaDto dto, CancellationToken ct = default)
        {
            var response = await _http.PostAsJsonAsync("api/formulas/import", dto, ct);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<ImportResult>(cancellationToken: ct);
                    return result ?? new ImportResult("Success", new List<string>());

                }
                catch (Exception ex) {
                    throw;

                }

            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                return new ImportResult("Failed", new List<string> { errorContent });
            }
        }

        public async Task<bool> Delete(string id, CancellationToken ct = default)
        {
            var response = await _http.DeleteAsync($"api/formulas/{id}", ct);
            return response.IsSuccessStatusCode;
        }

        public record FormulaListItem(
            string Id,
            string Name,
            decimal BatchWeight,
            string WeightUnit,
            decimal TotalCost,
            bool IsHighlighted);

        public record ImportResult(string Status, List<string> Messages);
    }
}