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
            return Ok(patio);
        }

        [HttpPost("{patioId}/motos")]
        public async Task<ActionResult<MotoDto>> AdicionarMoto(Guid patioId, [FromBody] MotoDto motoDto)
        {
            try
            {
                var moto = await _patioService.AdicionarMotoAsync(patioId, motoDto.Placa, motoDto.Posicao);
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
    }
}
