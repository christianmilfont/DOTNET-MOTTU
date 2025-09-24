using Microsoft.EntityFrameworkCore;
using Mottu_DOTNET.src.Application.Interfaces;
using Mottu_DOTNET.src.Domain.Entities;
using Mottu_DOTNET.src.Infrastructure.Data;

namespace Mottu_DOTNET.src.Infrastructure.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly AppDbContext _context;
        public MotoRepository(AppDbContext context) => _context = context;

        public async Task AdicionarAsync(Moto moto)
        {
            await _context.Motos.AddAsync(moto);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Moto>> ObterPaginadoAsync(int pageNumber, int pageSize)
        {
            return await _context.Motos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> ObterTotalAsync()
        {
            return await _context.Motos.CountAsync();
        }

        public async Task AtualizarAsync(Moto moto) { _context.Motos.Update(moto); await _context.SaveChangesAsync(); }
        public async Task RemoverAsync(Moto moto) { _context.Motos.Remove(moto); await _context.SaveChangesAsync(); }
        public async Task<Moto?> ObterPorIdAsync(Guid id) => await _context.Motos.FindAsync(id);
        public async Task<Moto?> ObterPorPlacaAsync(string placa) => await _context.Motos
    .Where(m => m.Placa.Numero == placa)
    .FirstOrDefaultAsync(); //Depois de configurar Owned Type a query funciona normalmente:
    }

   
}
