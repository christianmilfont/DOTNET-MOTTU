using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mottu_DOTNET.src.Application.DTOs;
using Mottu_DOTNET.src.Application.Services;

namespace Mottu_DOTNET.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatioController : ControllerBase
    {
        private readonly PatioService _patioService;

        public PatioController(PatioService patioService)
        {
            _patioService = patioService;
        }

        [HttpPost]
        [Authorize] //adicionando uma camada de proteção com JWT 
        public async Task<ActionResult<PatioDto>> CriarPatio([FromBody] string nome)
        {
            var patio = await _patioService.CriarPatioAsync(nome);
            return CreatedAtAction(nameof(ObterPatio), new { id = patio.Id }, patio);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatioDto>> ObterPatio(Guid id)
        {
            var patio = await _patioService.ObterPatioComMotosAsync(id);
            if (patio == null) return NotFound();

            AdicionarLinksPatio(patio);
            return Ok(patio);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatioDto>>> ObterPaginado([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var patios = await _patioService.ObterPatiosPaginadosAsync(pageNumber, pageSize);
            var total = await _patioService.ObterTotalPatiosAsync();

            // pode implementar um wrapper com links de paginação aqui (HATEOAS)
            Response.Headers.Append("X-Total-Count", total.ToString());

            foreach (var patio in patios)
                AdicionarLinksPatio(patio);

            return Ok(patios);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverPatio(Guid id)
        {
            try
            {
                await _patioService.RemoverPatioAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // Métodos para lidar com motos dentro do pátio (como antes)

        [HttpPost("{patioId}/motos")]
        public async Task<ActionResult<MotoDto>> AdicionarMoto(Guid patioId, [FromBody] MotoDto motoDto)
        {
            try
            {
                var moto = await _patioService.AdicionarMotoAsync(patioId, motoDto.Placa, motoDto.Posicao, motoDto.ClienteId);
                return CreatedAtAction(nameof(ObterPatio), new { id = patioId }, moto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{patioId}/motos/{placa}")]
        public async Task<IActionResult> RemoverMoto(Guid patioId, string placa)
        {
            try
            {
                await _patioService.RemoverMotoAsync(patioId, placa);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private void AdicionarLinksPatio(PatioDto patio)
        {
            // Exemplo simples de HATEOAS: adicionando links como headers ou dentro do DTO (se usar classe)
            // Aqui só um exemplo simples — você pode implementar um DTO que contenha Links
            // Exemplo: patio.Links = new List<LinkDto> { new LinkDto(...) };
        }
    }
}
