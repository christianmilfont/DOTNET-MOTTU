namespace Mottu_DOTNET.src.Api.Controllers.V1;
using Microsoft.AspNetCore.Mvc;
using Mottu_DOTNET.src.Application.Services.Auth;
using Mottu_DOTNET.src.Domain.Entities;
using Mottu_DOTNET.src.Infrastructure.Data;

using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public LoginController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost]
    public IActionResult Login([FromBody] Cliente request)
    {
        var cliente = _context.Clientes
            .FirstOrDefault(c => c.Email == request.Email && c.Nome == request.Nome); // considere adicionar senha depois

        if (cliente == null)
        {
            return Unauthorized("Credenciais inválidas");
        }

        var token = _jwtService.GenerateToken(cliente.Id, cliente.Email);

        return Ok(new
        {
            Token = token,
            Cliente = new
            {
                cliente.Id,
                cliente.Nome,
                cliente.Email,
                cliente.Telefone
            }
        });
    }
}
    

   

