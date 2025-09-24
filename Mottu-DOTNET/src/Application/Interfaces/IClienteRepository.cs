using Mottu_DOTNET.src.Domain.Entities;

namespace Mottu_DOTNET.src.Application.Interfaces
{
    public interface IClienteRepository
    {
        Task AdicionarAsync(Cliente cliente);
        Task AtualizarAsync(Cliente cliente);
        Task RemoverAsync(Cliente cliente);
        Task<Cliente?> ObterPorIdAsync(Guid id);
        Task<List<Cliente>> ObterPaginadoAsync(int pageNumber, int pageSize);
        Task<int> ObterTotalAsync();

    }

}
