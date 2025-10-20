using Microsoft.ML;
using Microsoft.ML.Data;
using Mottu_DOTNET.src.Domain.ML;
using System.Collections.Generic;

namespace Mottu_DOTNET.src.Domain.ML
{
    public class TextClassificationModel
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public TextClassificationModel()
        {
            _mlContext = new MLContext();
        }

        // Treina o modelo com perguntas e respostas
        public void TrainModel(IEnumerable<InputData> trainingData)
        {
            var trainData = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label")
                .Append(_mlContext.Transforms.Text.FeaturizeText("Features", "Text"))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedCategory", "PredictedLabel"));

            _model = pipeline.Fit(trainData);
        }

        // Faz a previsão e retorna a resposta associada
        public PredictionResult Predict(string inputText)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<InputData, PredictionResult>(_model);
            var input = new InputData { Text = inputText };
            var prediction = predictionEngine.Predict(input);

            // Aqui associamos a resposta baseada na previsão (com base na categoria)
            return prediction;
        }
    }

   
}
