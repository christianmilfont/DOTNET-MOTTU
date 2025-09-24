using Mottu_DOTNET.src.Domain.Entities;

namespace Mottu_DOTNET.src.Application.Interfaces
{
    public interface IPatioRepository
    {
        Task<Patio?> ObterPorIdAsync(Guid id);
      
        Task<List<Patio>> ObterPaginadoAsync(int pageNumber, int pageSize);
        Task<int> ObterTotalAsync();
        Task AdicionarAsync(Patio patio);
        Task AtualizarAsync(Patio patio);
        Task RemoverAsync(Patio patio);
    }
}
