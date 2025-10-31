using System.Net.Http;
using System.Net.Http.Headers; // 👈 necessário para AuthenticationHeaderValue
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vendas.API.Services
{
    public class EstoqueClient : IEstoqueClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<EstoqueClient> _logger;

        public EstoqueClient(HttpClient http, ILogger<EstoqueClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        // ✅ Adicionamos o parâmetro 'token'
        public async Task<ProductResponse?> GetProductAsync(int productId, string? token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/products/{productId}");

            // ✅ Agora o 'token' existe no escopo
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _http.SendAsync(request);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Erro ao consultar produto {ProductId} - Status {Status}", productId, resp.StatusCode);
                return null;
            }

            return await resp.Content.ReadFromJsonAsync<ProductResponse>();
        }

        public async Task<bool> UpdateProductQuantityAsync(int id, int newQuantity, string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = JsonSerializer.Serialize(new { quantity = newQuantity });
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _http.PutAsync($"api/products/{id}/quantity", content);
            return response.IsSuccessStatusCode;
        }
    }
}
