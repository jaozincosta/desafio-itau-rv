using System;
using System.Collections.Generic;
using InvestimentosRendaVariavel.DbContexto;
using InvestimentosRendaVariavel.Services;

class Program
{
    static void Main(string[] args)
    {
        using var context = new InvestimentoContext();
        var service = new InvestimentoService(context);

        int usuarioId = 1;
        int ativoId = 1;

        ExibirResumoUsuario(service, usuarioId, ativoId);

        // =============================
        // Simulação: Cálculo de Preço Médio Ponderado
        // =============================

        /*
        var calculadora = new CalculoPrecoMedioService();

        var compras = new List<(int, decimal)>
        {
            (100, 10.00m),
            (50, 12.00m),
            (150, 9.50m)
        };

        var precoMedio = calculadora.CalcularPrecoMedio(compras);
        Console.WriteLine($"\n[Simulação] Preço médio ponderado: R$ {precoMedio:F2}");
        */
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
