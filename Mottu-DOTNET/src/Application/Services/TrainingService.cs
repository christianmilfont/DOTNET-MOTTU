using Mottu_DOTNET.src.Domain.ML;
using System.Collections.Generic;

namespace Mottu_DOTNET.src.Application.Services
{
    public class TrainingService
    {
        private readonly TextClassificationModel _textClassificationModel;

        public TrainingService(TextClassificationModel textClassificationModel)
        {
            _textClassificationModel = textClassificationModel;
        }

        public void TrainModel()
        {
            // Dados de treinamento com perguntas e respostas
            var trainingData = new List<InputData>
            {
                new InputData { Text = "Quais são os defeitos comuns nas motos?", Resposta = "Os defeitos mais comuns em motos incluem falhas no motor, problemas na suspensão, e desgaste de pneus." },
                new InputData { Text = "Como faço para entrar em contato com o atendimento?", Resposta = "Você pode entrar em contato com nosso atendimento através do telefone (11) 1234-5678 ou pelo email atendimento@mottu.com." },
                new InputData { Text = "Qual é o processo de gestão de pátio?", Resposta = "A gestão de pátio inclui o controle do espaço para armazenamento das motos, a organização das motos para facilitar o acesso e o acompanhamento de entrada e saída." },
                new InputData { Text = "Meu motor está fazendo barulho estranho!", Resposta = "Se o motor da sua moto está fazendo barulho estranho, pode ser um problema com a vela de ignição ou até com o sistema de exaustão. É recomendado realizar uma verificação detalhada." },
                new InputData { Text = "Como fazer o reparo da suspensão da moto?", Resposta = "O reparo da suspensão envolve a substituição de peças danificadas, como amortecedores e molas. Certifique-se de realizar o reparo com um mecânico qualificado." },
                new InputData { Text = "Como agendar uma visita para manutenção?", Resposta = "Você pode agendar uma visita para manutenção pelo nosso site ou ligando diretamente para nossa central de atendimento." },
                // Adicionaremos depois mais exemplos de perguntas e respostas aqui
            };

            // Treinando o modelo com as perguntas e respostas
            _textClassificationModel.TrainModel(trainingData);
        }
    }
}
