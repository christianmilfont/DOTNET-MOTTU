using Microsoft.EntityFrameworkCore;
using Mottu_DOTNET.src.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Mottu_DOTNET.src.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Patio> Patios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patio>().HasMany(p => p.Motos).WithOne().OnDelete(DeleteBehavior.Cascade);
            // Configuração do VO Placa
            modelBuilder.Entity<Moto>().OwnsOne(m => m.Placa, placa =>
            {
                placa.Property(p => p.Numero).HasColumnName("Placa"); // coluna no banco
            });
            // Configuração do VO Posicao (enum)
            modelBuilder.Entity<Moto>().OwnsOne(m => m.Posicao, pos =>
            {
                pos.Property(p => p.Valor)
                   .HasColumnName("Posicao")
                   .HasConversion<string>();
            });
        }
    }
}
