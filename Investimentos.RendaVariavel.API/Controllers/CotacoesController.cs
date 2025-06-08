using Microsoft.AspNetCore.Mvc;
using InvestimentosRendaVariavel.Services;

namespace InvestimentosRendaVariavel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotacoesController : ControllerBase
    {
        private readonly CotacaoExternaService _cotacaoExternaService;

        public CotacoesController(CotacaoExternaService cotacaoExternaService)
        {
            _cotacaoExternaService = cotacaoExternaService;
        }

        [HttpGet("{codigoAtivo}")]
        public async Task<IActionResult> ObterUltimaCotacao(string codigoAtivo)
        {
            var preco = await _cotacaoExternaService.ObterUltimaCotacaoAsync(codigoAtivo);

            if (preco == null)
                return NotFound("Cotação não disponível no momento.");

            return Ok(new
            {
                Ativo = codigoAtivo,
                Preco = preco
            });
        }
    }
}
