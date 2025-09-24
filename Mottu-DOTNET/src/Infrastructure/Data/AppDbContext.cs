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
        public DbSet<Cliente> Clientes { get; set; } // <<< Adicionado

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relação Patio -> Motos (1:n) com Cascade Delete
            modelBuilder.Entity<Patio>()
                .HasMany(p => p.Motos)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Relação Cliente -> Motos (1:n) Opcional
            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Motos)
                .WithOne(m => m.Cliente)
                .HasForeignKey(m => m.ClienteId)
                .IsRequired(false) // Torna a FK opcional
                .OnDelete(DeleteBehavior.SetNull); // Opcional: quando cliente deletado, desvincula motos

            // Configuração do VO Placa
            modelBuilder.Entity<Moto>().OwnsOne(m => m.Placa, placa =>
            {
                placa.Property(p => p.Numero).HasColumnName("Placa");
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
