using System.Numerics;
using Mottu_DOTNET.src.Domain.ValueObjects;

namespace Mottu_DOTNET.src.Domain.Entities
{
    public class Moto
    {
        public Guid Id { get; private set; }
        public Placa Placa { get; private set; }
        public string Status { get; private set; }
        public Posicao Posicao { get; private set; }
        // Nova propriedade para associar cliente (opcional)
        public Guid? ClienteId { get; private set; }
        public Cliente? Cliente { get; private set; }

        protected Moto() { } // EF Core
        //O construtor protegido permite que o EF Core crie a instância apenas para persistência, sem expor ao domínio.
        public Moto(Placa placa, Posicao posicao)
        {
            Id = Guid.NewGuid();
            Placa = placa;
            Posicao = posicao;
            Status = "Pronta";
        }

        public void AlterarStatus(string novoStatus)
        {
            Status = novoStatus;
        }

        public void AlterarPosicao(Posicao novaPosicao)
        {
            Posicao = novaPosicao;
        }

        // Método para associar cliente
        public void AssociarCliente(Cliente cliente)
        {
            Cliente = cliente;
            ClienteId = cliente.Id;
        }

        // Método para remover associação
        public void RemoverCliente()
        {
            Cliente = null;
            ClienteId = null;
        }
    }

}
