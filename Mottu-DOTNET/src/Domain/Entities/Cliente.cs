namespace Mottu_DOTNET.src.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Telefone { get; private set; }
        public string Email { get; private set; }
        public string Endereco { get; private set; }

        // Um cliente pode ter várias motos, ou nenhuma
        public ICollection<Moto> Motos { get; private set; }

        protected Cliente() { } // EF Core

        public Cliente(string nome, string telefone, string email, string endereco)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Telefone = telefone;
            Email = email;
            Endereco = endereco;
            Motos = new List<Moto>();
        }

        // Métodos para manipular motos associadas, se quiser
        public void AdicionarMoto(Moto moto)
        {
            if (!Motos.Contains(moto))
            {
                Motos.Add(moto);
                moto.AssociarCliente(this);
            }
        }

        public void RemoverMoto(Moto moto)
        {
            if (Motos.Contains(moto))
            {
                Motos.Remove(moto);
                moto.RemoverCliente();
            }
        }
        // Método para atualizar dados
        public void AtualizarDados(string nome, string telefone, string email, string endereco)
        {
            Nome = nome;
            Telefone = telefone;
            Email = email;
            Endereco = endereco;
        }
    }
}
