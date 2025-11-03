using Mottu_DOTNET.src.Application.Services;
using Mottu_DOTNET.src.Domain.ML;
using System;
using Xunit;

namespace Mottu_DOTNET.Tests.Integration
{
    public class TrainingServiceIntegrationTests
    {
        [Fact]
        public void TrainModel_ShouldCallTextClassificationModel()
        {
            // Arrange: criar instância do modelo real
            var textModel = new TextClassificationModel();
            var trainingService = new TrainingService(textModel);

            // Act: treinar o modelo
            Exception ex = Record.Exception(() => trainingService.TrainModel());

            // Assert: não deve lançar nenhuma exceção
            Assert.Null(ex);
        }
    }
}
