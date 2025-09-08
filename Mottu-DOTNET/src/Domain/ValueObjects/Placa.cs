namespace Mottu_DOTNET.src.Domain.ValueObjects
{
    public record Placa
    {
        public string Numero { get; }

        protected Placa() { } // Para EF Core
        public Placa(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("Placa inválida");
            Numero = numero;
        }
    }
}
