using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using InvestimentosRendaVariavel.DbContexto;
using InvestimentosRendaVariavel.Services;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Configura DI
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<InvestimentoContext>(options =>
            options.UseMySql(
                "server=localhost;database=investimentosdb;user=root;password=1234",
                ServerVersion.AutoDetect("server=localhost;database=investimentosdb;user=root;password=1234")
            ));

        serviceCollection.AddScoped<InvestimentoService>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<InvestimentoService>();

        int usuarioId = 1;
        int ativoId = 1;

        ExibirResumoUsuario(service, usuarioId, ativoId);
    }

    static void ExibirResumoUsuario(InvestimentoService service, int usuarioId, int ativoId)
    {
        Console.WriteLine("Resumo do usuário:");
        Console.WriteLine("-------------------");

        var totalInvestido = service.CalcularTotalInvestidoPorAtivo(usuarioId, ativoId);
        var totalCorretagem = service.CalcularTotalCorretagemPorUsuario(usuarioId);
        var pnlGlobal = service.CalcularPnLGlobal(usuarioId);
        var posicoes = service.ObterPosicoesPorUsuario(usuarioId);

        Console.WriteLine($"Total investido no ativo {ativoId}: R$ {totalInvestido:F2}");
        Console.WriteLine($"Total de corretagem: R$ {totalCorretagem:F2}");
        Console.WriteLine($"PnL global: R$ {pnlGlobal:F2}");

        Console.WriteLine("\nPosições:");
        foreach (var pos in posicoes)
        {
            Console.WriteLine($"AtivoId: {pos.AtivoId}, Quantidade: {pos.Quantidade}, Preço Médio: {pos.PrecoMedio}, PnL: {pos.PnL}");
        }
    }
}
