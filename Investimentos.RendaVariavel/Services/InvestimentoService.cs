using InvestimentosRendaVariavel.DbContexto;
using InvestimentosRendaVariavel.Models;
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
	}
}