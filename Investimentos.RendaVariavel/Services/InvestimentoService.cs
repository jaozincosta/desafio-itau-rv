using InvestimentosRendaVariavel.DbContexto;
using InvestimentosRendaVariavel.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosRendaVariavel.Services
{
    public class InvestimentoService
    {
        private readonly InvestimentoContext _context;

        public InvestimentoService(InvestimentoContext context)
        {
            _context = context;
        }

        public decimal CalcularTotalInvestidoPorAtivo(int usuarioId, int ativoId)
        {
            return _context.Operacoes
                .Where(o => o.UsuarioId == usuarioId && o.AtivoId == ativoId && o.TipoOperacao == "Compra")
                .Sum(o => o.PrecoUnitario * o.Quantidade);
        }

        public decimal CalcularTotalCorretagemPorUsuario(int usuarioId)
        {
            return _context.Operacoes
                .Where(o => o.UsuarioId == usuarioId)
                .Sum(o => o.Corretagem);
        }

        public decimal CalcularTotalCorretagemGlobal()
        {
            return _context.Operacoes.Sum(o => o.Corretagem);
        }

        public decimal CalcularPnLGlobal(int usuarioId)
        {
            return _context.Posicoes
                .Where(p => p.UsuarioId == usuarioId)
                .Sum(p => p.PnL);
        }

        public IEnumerable<Posicao> ObterPosicoesPorUsuario(int usuarioId)
        {
            return _context.Posicoes
                .Where(p => p.UsuarioId == usuarioId)
                .ToList();
        }

        public IEnumerable<object> ObterTop10UsuariosPorPosicao()
        {
            return _context.Posicoes
                .GroupBy(p => p.UsuarioId)
                .Select(g => new
                {
                    UsuarioId = g.Key,
                    TotalPosicao = g.Sum(p => p.Quantidade * p.PrecoMedio)
                })
                .OrderByDescending(x => x.TotalPosicao)
                .Take(10)
                .ToList();
        }

        public IEnumerable<object> ObterTop10UsuariosPorCorretagem()
        {
            return _context.Operacoes
                .GroupBy(o => o.UsuarioId)
                .Select(g => new
                {
                    UsuarioId = g.Key,
                    TotalCorretagem = g.Sum(o => o.Corretagem)
                })
                .OrderByDescending(x => x.TotalCorretagem)
                .Take(10)
                .ToList();
        }
    }
}
