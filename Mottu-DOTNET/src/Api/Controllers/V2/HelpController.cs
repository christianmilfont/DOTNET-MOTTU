using Microsoft.AspNetCore.Mvc;
using Mottu_DOTNET.src.Application.Services;
using Mottu_DOTNET.src.Domain.ML;

namespace Mottu_DOTNET.src.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelpController : ControllerBase
    {
        private readonly TrainingService _trainingService;
        private readonly TextClassificationModel _textClassificationModel;

        public HelpController(TrainingService trainingService, TextClassificationModel textClassificationModel)
        {
            _trainingService = trainingService;
            _textClassificationModel = textClassificationModel;
        }

        // Endpoint para treinar o modelo
        [HttpPost("train")]
        public IActionResult TrainModel()
        {
            _trainingService.TrainModel();
            return Ok("Modelo treinado com sucesso!");
        }

        // Endpoint para classificar um texto e retornar a resposta associada
        [HttpPost("classify")]
        public IActionResult Classify([FromBody] InputData inputData)
        {
            if (inputData == null)
                return BadRequest("Texto de entrada inválido.");

            var result = _textClassificationModel.Predict(inputData.Text);
            return Ok(new { PredictedCategory = result.PredictedCategory, PredictedResponse = result.PredictedResponse });
        }
    }
}
