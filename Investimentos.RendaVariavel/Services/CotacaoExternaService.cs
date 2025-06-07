using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.CircuitBreaker;

namespace InvestimentosRendaVariavel.Services
{
    public class CotacaoExternaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CotacaoExternaService> _logger;
        private readonly IAsyncPolicy<HttpResponseMessage> _policyComposta;

        public CotacaoExternaService(HttpClient httpClient, ILogger<CotacaoExternaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => !msg.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2),
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("Tentativa {RetryCount} falhou. Retentando em {Delay}s...", retryCount, timeSpan.TotalSeconds);
                    });

            var circuitBreakerPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(10),
                    onBreak: (outcome, breakDelay) =>
                    {
                        _logger.LogWarning("Circuit Breaker ATIVO por {BreakDelay}s devido a falhas consecutivas.", breakDelay.TotalSeconds);
                    },
                    onReset: () => _logger.LogInformation("Circuit Breaker REINICIADO."));

            _policyComposta = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        }

        public async Task<decimal?> ObterUltimaCotacaoAsync(string codigoAtivo)
        {
            try
            {
                var response = await _policyComposta.ExecuteAsync(() =>
                    _httpClient.GetAsync($"https://b3api.vercel.app/quote/{codigoAtivo}"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Falha ao obter cotação para {CodigoAtivo}. Status: {StatusCode}", codigoAtivo, response.StatusCode);
                    return null;
                }

                using var content = await response.Content.ReadAsStreamAsync();
                var json = await JsonDocument.ParseAsync(content);

                if (json.RootElement.TryGetProperty("regularMarketPrice", out var preco))
                {
                    return preco.GetDecimal();
                }

                _logger.LogWarning("Resposta não contém campo 'regularMarketPrice'.");
                return null;
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit breaker está aberto. Fallback ativado para {CodigoAtivo}.", codigoAtivo);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter cotação externa.");
                return null;
            }
        }
    }
}
