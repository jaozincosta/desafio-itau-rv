using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using InvestimentosRendaVariavel.DbContexto;
using InvestimentosRendaVariavel.Models;

namespace InvestimentosRendaVariavel.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mensagensSimuladas = new List<CotacaoMensagem>
            {
                new CotacaoMensagem { AtivoId = 1, PrecoUnitario = 10.50m, DataHora = DateTime.Now.AddSeconds(-10) },
                new CotacaoMensagem { AtivoId = 2, PrecoUnitario = 11.25m, DataHora = DateTime.Now.AddSeconds(-5) },
                new CotacaoMensagem { AtivoId = 1, PrecoUnitario = 10.75m, DataHora = DateTime.Now } // duplicada simulada
            };

            foreach (var message in mensagensSimuladas)
            {
                try
                {
                    using var context = new InvestimentoContext();

                    var jaExiste = context.Cotacoes.Any(c =>
                        c.AtivoId == message.AtivoId && c.DataHora == message.DataHora);

                    if (!jaExiste)
                    {
                        context.Cotacoes.Add(new Cotacao
                        {
                            AtivoId = message.AtivoId,
                            PrecoUnitario = message.PrecoUnitario,
                            DataHora = message.DataHora
                        });

                        await context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"[OK] Cotação salva: AtivoId={message.AtivoId}, Preço={message.PrecoUnitario}");
                    }
                    else
                    {
                        _logger.LogInformation("[IGNORADA] Cotação duplicada para mesmo Ativo e horário.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao salvar cotação. Retentando em 2s...");
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
