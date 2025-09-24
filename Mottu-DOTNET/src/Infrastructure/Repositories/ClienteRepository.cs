using Microsoft.EntityFrameworkCore;
using Mottu_DOTNET.src.Application.Interfaces;
using Mottu_DOTNET.src.Domain.Entities;
using Mottu_DOTNET.src.Infrastructure.Data;

namespace Mottu_DOTNET.src.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Cliente cliente)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task<Cliente?> ObterPorIdAsync(Guid id)
        {
            // Incluindo as motos do cliente para facilitar navegação, se desejar
            return await _context.Clientes
                .Include(c => c.Motos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<List<Cliente>> ObterPaginadoAsync(int pageNumber, int pageSize)
        {
            return await _context.Clientes
                .Include(c => c.Motos)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> ObterTotalAsync()
        {
            return await _context.Clientes.CountAsync();
        }

    }
}
