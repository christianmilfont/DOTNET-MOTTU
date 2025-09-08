namespace Mottu_DOTNET.src.Domain.ValueObjects
{
    public class Posicao
    {
        public TipoPosicao Valor { get; private set; }

        protected Posicao() { }


        public Posicao(TipoPosicao valor)
        {
            Valor = valor;
        }

        public override string ToString() => Valor.ToString();

        public enum TipoPosicao
        {
            Front, // Cliente trouxe a moto
            Back   // Empresa trouxe por inadimplência
        }

        // Igualdade de Value Object
        public override bool Equals(object? obj)
        {
            if (obj is not Posicao other) return false;
            return Valor == other.Valor;
        }

        public override int GetHashCode() => Valor.GetHashCode();
    }
}
