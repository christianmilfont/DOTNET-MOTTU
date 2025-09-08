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
    }

}
