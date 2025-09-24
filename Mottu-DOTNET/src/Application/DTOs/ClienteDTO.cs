namespace Mottu_DOTNET.src.Application.DTOs
{
    public record ClienteDto(
       Guid Id,
       string Nome,
       string Telefone,
       string Email,
       string Endereco,
       List<Guid>? MotoIds  // IDs das motos associadas (opcional)
   );
}
