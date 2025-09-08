using Mottu_DOTNET.src.Domain.Entities;

namespace Mottu_DOTNET.src.Application.Interfaces
{
    public interface IMotoRepository
    {
        Task<Moto?> ObterPorIdAsync(Guid id);
        Task<Moto?> ObterPorPlacaAsync(string placa);
        Task AdicionarAsync(Moto moto);
        Task AtualizarAsync(Moto moto);
        Task RemoverAsync(Moto moto);
    }
}
