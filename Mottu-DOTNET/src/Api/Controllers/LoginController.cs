namespace Mottu_DOTNET.src.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Mottu_DOTNET.src.Domain.Entities;
using Mottu_DOTNET.src.Infrastructure.Data;
using System.Linq;


    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Login([FromBody] Cliente request)
        {
            var cliente = _context.Clientes
                .FirstOrDefault(c => c.Email == request.Email && c.Nome == request.Nome); 

            if (cliente == null)
            {
                return Unauthorized("Credenciais inválidas");
            }

            return Ok(cliente);
        }
    }

   

