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

        // POST: api/moto/adicionar
        [HttpPost("adicionar")]
        public async Task<ActionResult<MotoDto>> AdicionarMoto([FromBody] AdicionarMotoRequest request)
        {
            try
            {
                var motoDto = await _patioService.AdicionarMotoAsync(request.PatioId, request.Placa, request.Posicao, request.ClienteId);
                return CreatedAtAction(nameof(ObterPorPlaca), new { placa = motoDto.Placa }, motoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/moto/{placa}
        [HttpGet("{placa}")]
        public async Task<ActionResult<MotoDto>> ObterPorPlaca(string placa)
        {
            var motoDto = await _patioService.ObterMotoPorPlacaAsync(placa);
            if (motoDto == null) return NotFound();
            return Ok(motoDto);
        }

        // PUT: api/moto/atualizar/{placa}
        [HttpPut("atualizar/{placa}")]
        public async Task<ActionResult<MotoDto>> AtualizarMoto(string placa, [FromBody] AtualizarMotoRequest request)
        {
            var motoDto = await _patioService.AtualizarMotoAsync(placa, request.Status, request.Posicao);
            if (motoDto == null) return NotFound();
            return Ok(motoDto);
        }

        // DELETE: api/moto/remover/{patioId}/{placa}
        [HttpDelete("remover/{patioId}/{placa}")]
        public async Task<ActionResult> RemoverMoto(Guid patioId, string placa)
        {
            try
            {
                await _patioService.RemoverMotoAsync(patioId, placa);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/moto/listar?pageNumber=1&pageSize=10
        [HttpGet("listar")]
        public async Task<ActionResult<List<MotoDto>>> ListarMotosPaginadas([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var motos = await _patioService.ObterMotosPaginadasAsync(pageNumber, pageSize);
            return Ok(motos);
        }

        // GET: api/moto/total
        [HttpGet("total")]
        public async Task<ActionResult<int>> ObterTotalMotos()
        {
            var total = await _patioService.ObterTotalMotosAsync();
            return Ok(total);
        }

        // DTOs para requisições
        public class AdicionarMotoRequest
        {
            public Guid PatioId { get; set; }
            public string Placa { get; set; } = null!;
            public string Posicao { get; set; } = null!;
            public Guid? ClienteId { get; set; }
        }

        public class AtualizarMotoRequest
        {
            public string Status { get; set; } = null!;
            public string Posicao { get; set; } = null!;
        }
    }
}
