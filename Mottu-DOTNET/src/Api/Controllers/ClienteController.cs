using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mottu_DOTNET.src.Application.DTOs;
using Mottu_DOTNET.src.Application.Interfaces;
using Mottu_DOTNET.src.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mottu_DOTNET.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly LinkGenerator _linkGenerator;

        public ClienteController(IClienteRepository clienteRepository, LinkGenerator linkGenerator)
        {
            _clienteRepository = clienteRepository;
            _linkGenerator = linkGenerator;
        }

        // GET: api/cliente?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetClientes(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0) return BadRequest("pageNumber e pageSize devem ser maiores que zero");

            // Aqui vai chamar o método que retorne clientes paginados (no repository)
            var clientesPaged = await _clienteRepository.ObterPaginadoAsync(pageNumber, pageSize);

            var totalClientes = await _clienteRepository.ObterTotalAsync();

            var totalPages = (int)Math.Ceiling(totalClientes / (double)pageSize);

            var clientesDto = clientesPaged.Select(cliente => new ClienteDto(
                cliente.Id,
                cliente.Nome,
                cliente.Telefone,
                cliente.Email,
                cliente.Endereco,
                cliente.Motos?.Select(m => m.Id).ToList()
            )).ToList();

            var response = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalClientes,
                Data = clientesDto.Select(c => CriarLinksCliente(c))
            };

            // Links HATEOAS gerais
            var links = new List<object>
            {
                new { rel = "self", href = _linkGenerator.GetPathByAction(HttpContext, nameof(GetClientes), values: new { pageNumber, pageSize }) }
            };
            if (pageNumber < totalPages)
                links.Add(new { rel = "next", href = _linkGenerator.GetPathByAction(HttpContext, nameof(GetClientes), values: new { pageNumber = pageNumber + 1, pageSize }) });
            if (pageNumber > 1)
                links.Add(new { rel = "prev", href = _linkGenerator.GetPathByAction(HttpContext, nameof(GetClientes), values: new { pageNumber = pageNumber - 1, pageSize }) });

            return Ok(new { metadata = response, links });
        }

        // GET api/cliente/{id}
        [HttpGet("{id:guid}", Name = nameof(GetClienteById))]
        public async Task<IActionResult> GetClienteById(Guid id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null) return NotFound();

            var clienteDto = new ClienteDto(
                cliente.Id,
                cliente.Nome,
                cliente.Telefone,
                cliente.Email,
                cliente.Endereco,
                cliente.Motos?.Select(m => m.Id).ToList()
            );

            return Ok(CriarLinksCliente(clienteDto));
        }

        // POST api/cliente
        [HttpPost]
        public async Task<IActionResult> CriarCliente([FromBody] ClienteDto clienteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cliente = new Cliente(
                clienteDto.Nome,
                clienteDto.Telefone,
                clienteDto.Email,
                clienteDto.Endereco
            );

            await _clienteRepository.AdicionarAsync(cliente);

            var createdDto = clienteDto with { Id = cliente.Id };

            return CreatedAtRoute(nameof(GetClienteById), new { id = cliente.Id }, CriarLinksCliente(createdDto));
        }

        // PUT api/cliente/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> AtualizarCliente(Guid id, [FromBody] ClienteDto clienteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != clienteDto.Id) return BadRequest("IDs não conferem");

            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null) return NotFound();

            cliente.AtualizarDados(clienteDto.Nome, clienteDto.Telefone, clienteDto.Email, clienteDto.Endereco);
            await _clienteRepository.AtualizarAsync(cliente);

            return NoContent();
        }

        // DELETE api/cliente/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletarCliente(Guid id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null) return NotFound();

            await _clienteRepository.RemoverAsync(cliente);

            return NoContent();
        }

        // Método para criar links HATEOAS no DTO
        private object CriarLinksCliente(ClienteDto cliente)
        {
            var links = new List<object>
            {
                new { rel = "self", href = _linkGenerator.GetPathByAction(HttpContext, nameof(GetClienteById), values: new { id = cliente.Id }) },
                new { rel = "update", href = _linkGenerator.GetPathByAction(HttpContext, nameof(AtualizarCliente), values: new { id = cliente.Id }) },
                new { rel = "delete", href = _linkGenerator.GetPathByAction(HttpContext, nameof(DeletarCliente), values: new { id = cliente.Id }) }
            };

            return new { cliente, links };
        }
    }
}
