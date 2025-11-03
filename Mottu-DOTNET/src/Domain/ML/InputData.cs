using Microsoft.ML.Data;

namespace Mottu_DOTNET.src.Domain.ML
{
    public class InputData
    {
        [LoadColumn(0)]
        public string Text { get; set; } = null!;  // Pergunta

        [LoadColumn(1)]
        [ColumnName("Label")] // ML.NET vai tratar "Resposta" como "Label"
        public string Resposta { get; set; } = null!; // Resposta associada
    }
}
