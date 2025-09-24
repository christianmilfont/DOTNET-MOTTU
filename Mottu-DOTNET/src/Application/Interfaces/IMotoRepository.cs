using Mottu_DOTNET.src.Domain.Entities;

namespace Mottu_DOTNET.src.Application.Interfaces
{
    public interface IMotoRepository
    {
        Task<Moto?> ObterPorIdAsync(Guid id);
        Task<Moto?> ObterPorPlacaAsync(string placa);
        Task<List<Moto>> ObterPaginadoAsync(int pageNumber, int pageSize);
        Task<int> ObterTotalAsync();
        Task AdicionarAsync(Moto moto);
        Task AtualizarAsync(Moto moto);
        Task RemoverAsync(Moto moto);
    }
}
