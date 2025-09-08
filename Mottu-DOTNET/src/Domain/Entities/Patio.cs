namespace Mottu_DOTNET.src.Domain.Entities
{
    public class Patio
    {
        private readonly List<Moto> _motos = new();

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public IReadOnlyCollection<Moto> Motos => _motos.AsReadOnly();

        protected Patio() { } // EF Core
        //O construtor protegido permite que o EF Core crie a instância apenas para persistência, sem expor ao domínio.

        public Patio(string nome)
        {
            Id = Guid.NewGuid();
            Nome = nome;
        }

        public void AdicionarMoto(Moto moto)
        {
            _motos.Add(moto);
        }

        public void RemoverMoto(Moto moto)
        {
            _motos.Remove(moto);
        }

        public Moto? BuscarPorPlaca(string placa)
        {
            return _motos.FirstOrDefault(m => m.Placa.Numero == placa);
        }
    }
}
