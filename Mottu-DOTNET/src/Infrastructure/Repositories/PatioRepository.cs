using Microsoft.EntityFrameworkCore;
using Mottu_DOTNET.src.Application.Interfaces;
using Mottu_DOTNET.src.Domain.Entities;
using Mottu_DOTNET.src.Infrastructure.Data;

namespace Mottu_DOTNET.src.Infrastructure.Repositories
{
    public class PatioRepository : IPatioRepository
    {
        private readonly AppDbContext _context;
        public PatioRepository(AppDbContext context) => _context = context;

        public async Task AdicionarAsync(Patio patio)
        {
            await _context.Patios.AddAsync(patio);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Patio patio) { _context.Patios.Update(patio); await _context.SaveChangesAsync(); }
        public async Task RemoverAsync(Patio patio)
        {
            _context.Patios.Remove(patio);
            await _context.SaveChangesAsync();
        }

        public async Task<Patio?> ObterPorIdAsync(Guid id)
        {
            return await _context.Patios
                .Include(p => p.Motos)  // Inclui motos relacionadas
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Patio>> ObterPaginadoAsync(int pageNumber, int pageSize)
        {
            return await _context.Patios
                .Include(p => p.Motos)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> ObterTotalAsync()
        {
            return await _context.Patios.CountAsync();
        }
    }
}
