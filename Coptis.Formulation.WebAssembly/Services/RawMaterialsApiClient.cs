// COMPLETE FILE - Replace ENTIRE contents of:
// Coptis.Formulation.WebAssembly/Services/RawMaterialsApiClient.cs

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Coptis.Formulation.WebAssembly.Services
{
    public sealed class RawMaterialsApiClient
    {
        private readonly HttpClient _http;

        public RawMaterialsApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<IReadOnlyList<RawMaterialListItem>> GetAll(CancellationToken ct = default)
        {
            try
            {
                var list = await _http.GetFromJsonAsync<List<RawMaterialListItem>>("api/RawMaterials", ct);
                return list ?? new List<RawMaterialListItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching raw materials: {ex.Message}");
                return new List<RawMaterialListItem>();
            }
        }

        public async Task<UpdatePriceResponse> UpdatePrice(string id, decimal newPrice, CancellationToken ct = default)
        {
            try
            {
                var request = new { PriceAmount = newPrice };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _http.PutAsync($"api/RawMaterials/{id}/price", content, ct);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UpdatePriceResponse>(cancellationToken: ct);
                    return result ?? new UpdatePriceResponse(false, "Unknown error", 0);
                }

                var error = await response.Content.ReadAsStringAsync(ct);
                return new UpdatePriceResponse(false, error, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating price: {ex.Message}");
                return new UpdatePriceResponse(false, ex.Message, 0);
            }
        }

        public record RawMaterialListItem(
            string Id,
            string Name,
            decimal PriceAmount,
            string Currency,
            string ReferenceUnit);

        public record UpdatePriceResponse(
            bool Success,
            string Message,
            int AffectedFormulasCount);
    }
}