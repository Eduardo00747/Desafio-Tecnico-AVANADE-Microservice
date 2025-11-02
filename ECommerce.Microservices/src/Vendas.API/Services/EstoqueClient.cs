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
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/products/{productId}");

                if (!string.IsNullOrEmpty(token))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resp = await _http.SendAsync(request);

                if (!resp.IsSuccessStatusCode)
                {
                    var errorContent = await resp.Content.ReadAsStringAsync();
                    _logger.LogWarning("Erro ao consultar produto {ProductId} - Status {Status} - Erro: {Error}", 
                        productId, resp.StatusCode, errorContent);
                    return null;
                }

                return await resp.Content.ReadFromJsonAsync<ProductResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar produto {ProductId}", productId);
                return null;
            }
        }

        public async Task<bool> UpdateProductQuantityAsync(int id, int newQuantity, string token)
        {
            try
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var updateDto = new { quantity = newQuantity };
                var response = await _http.PutAsJsonAsync($"/api/products/{id}/quantity", updateDto);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro ao atualizar quantidade do produto {ProductId}. Status: {Status}, Erro: {Error}", 
                        id, response.StatusCode, errorContent);
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar quantidade do produto {ProductId}", id);
                return false;
            }
        }
    }
}
