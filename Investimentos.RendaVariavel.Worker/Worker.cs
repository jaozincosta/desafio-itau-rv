using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using InvestimentosRendaVariavel.DbContexto;
using InvestimentosRendaVariavel.Models;
using InvestimentosRendaVariavel.Services;

namespace InvestimentosRendaVariavel.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CotacaoExternaService _cotacaoService;
        private readonly IServiceProvider _serviceProvider;

        public Worker(
            ILogger<Worker> logger,
            CotacaoExternaService cotacaoService,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _cotacaoService = cotacaoService;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mensagensSimuladas = new[]
            {
                new CotacaoMensagem { AtivoId = 1, PrecoUnitario = 10.50m, DataHora = DateTime.Now.AddSeconds(-10) },
                new CotacaoMensagem { AtivoId = 1, PrecoUnitario = 10.75m, DataHora = DateTime.Now }
            };

            foreach (var msg in mensagensSimuladas)
            {
                try
                {
                    var precoApi = await _cotacaoService.ObterUltimaCotacaoAsync("ITUB4");
                    if (precoApi.HasValue)
                    {
                        _logger.LogInformation("[API] Cotação externa: {Preco}", precoApi.Value);
                    }
                    else
                    {
                        _logger.LogWarning("[FALLBACK] Não foi possível obter cotação externa.");
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<InvestimentoContext>();

                    var existe = context.Cotacoes.Any(c =>
                        c.AtivoId == msg.AtivoId && c.DataHora == msg.DataHora);

                    if (!existe)
                    {
                        context.Cotacoes.Add(new Cotacao
                        {
                            AtivoId = msg.AtivoId,
                            PrecoUnitario = msg.PrecoUnitario,
                            DataHora = msg.DataHora
                        });

                        await context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("[OK] Cotação salva: AtivoId={AtivoId}, Preço={Preco}",
                            msg.AtivoId, msg.PrecoUnitario);
                    }
                    else
                    {
                        _logger.LogInformation("[SKIP] Cotação duplicada ignorada.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar cotação. Tentando novamente em 2s...");
                    await Task.Delay(2000, stoppingToken);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public class CotacaoMensagem
    {
        public int AtivoId { get; set; }
        public decimal PrecoUnitario { get; set; }
        public DateTime DataHora { get; set; }
    }
}
