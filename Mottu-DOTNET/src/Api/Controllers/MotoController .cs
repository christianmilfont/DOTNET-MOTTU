using Microsoft.AspNetCore.Mvc;
using Mottu_DOTNET.src.Application.DTOs;
using Mottu_DOTNET.src.Application.Services;

namespace Mottu_DOTNET.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotoController : ControllerBase
    {
        private readonly PatioService _patioService;

        public MotoController(PatioService patioService)
        {
            _patioService = patioService;
        }

        [HttpPatch("{placa}/status")]
        public async Task<ActionResult<MotoDto>> AlterarStatus(string placa, [FromBody] string novoStatus)
        {
            var moto = await _patioService.AlterarStatusMotoAsync(placa, novoStatus);
            if (moto == null) return NotFound();
            return Ok(moto);
        }

        [HttpGet("{placa}")]
        public async Task<ActionResult<MotoDto>> ObterPorPlaca(string placa)
        {
            // Criamos um método específico de consulta para não usar AlterarStatus
            var moto = await _patioService.ObterMotoPorPlacaAsync(placa);
            if (moto == null) return NotFound();
            return Ok(moto);
        }
    }
}
