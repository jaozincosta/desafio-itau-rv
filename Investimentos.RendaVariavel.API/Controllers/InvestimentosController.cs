using Microsoft.AspNetCore.Mvc;
using InvestimentosRendaVariavel.Services;

namespace InvestimentosRendaVariavel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvestimentosController : ControllerBase
    {
        private readonly InvestimentoService _service;

        public InvestimentosController(InvestimentoService service)
        {
            _service = service;
        }

        [HttpGet("preco-medio")]
        public IActionResult GetPrecoMedio([FromQuery] int usuarioId, [FromQuery] int ativoId)
        {
            var posicao = _service.ObterPosicoesPorUsuario(usuarioId)
                .FirstOrDefault(p => p.AtivoId == ativoId);

            if (posicao == null || posicao.Quantidade == 0)
                return NotFound("Posição não encontrada ou quantidade inválida.");

            var totalInvestido = _service.CalcularTotalInvestidoPorAtivo(usuarioId, ativoId);
            var preco = totalInvestido / posicao.Quantidade;

            return Ok(new { usuarioId, ativoId, precoMedio = preco });
        }

        [HttpGet("posicao/{usuarioId}")]
        public IActionResult GetPosicao(int usuarioId)
        {
            var posicoes = _service.ObterPosicoesPorUsuario(usuarioId);
            if (!posicoes.Any())
                return NotFound("Usuário não possui posições registradas.");

            return Ok(posicoes);
        }

        [HttpGet("corretagens/{usuarioId}")]
        public IActionResult GetCorretagens(int usuarioId)
        {
            var total = _service.CalcularTotalCorretagemPorUsuario(usuarioId);
            return Ok(new { usuarioId, totalCorretagem = total });
        }

        [HttpGet("ranking/top10-posicoes")]
        public IActionResult GetTop10Posicoes()
        {
            var ranking = _service.ObterTop10UsuariosPorPosicao();
            return Ok(ranking);
        }

        [HttpGet("ranking/top10-corretagens")]
        public IActionResult GetTop10Corretagens()
        {
            var ranking = _service.ObterTop10UsuariosPorCorretagem();
            return Ok(ranking);
        }

        [HttpGet("corretagem")]
        public IActionResult GetTotalCorretagemGlobal()
        {
            var total = _service.CalcularTotalCorretagemGlobal();
            return Ok(new { totalCorretagem = total });
        }
    }
}
