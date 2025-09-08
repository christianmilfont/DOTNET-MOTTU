using Mottu_DOTNET.src.Application.DTOs;
using Mottu_DOTNET.src.Application.Interfaces;
using Mottu_DOTNET.src.Domain.Entities;
using Mottu_DOTNET.src.Domain.ValueObjects;

namespace Mottu_DOTNET.src.Application.Services
{
    public class PatioService
    {
        private readonly IPatioRepository _patioRepository;
        private readonly IMotoRepository _motoRepository;

        public PatioService(IPatioRepository patioRepository, IMotoRepository motoRepository)
        {
            _patioRepository = patioRepository;
            _motoRepository = motoRepository;
        }

        public async Task<PatioDto> CriarPatioAsync(string nome)
        {
            var patio = new Patio(nome);
            await _patioRepository.AdicionarAsync(patio);
            return new PatioDto(patio.Id, patio.Nome, new List<MotoDto>());
        }

        public async Task<MotoDto> AdicionarMotoAsync(Guid patioId, string placa, string posicao)
        {
            var patio = await _patioRepository.ObterPorIdAsync(patioId);
            if (patio == null) throw new Exception("Pátio não encontrado");

            if (!Enum.TryParse<Posicao.TipoPosicao>(posicao, ignoreCase: true, out var tipoPosicao))
                throw new Exception("Tipo de posição inválido");

            var moto = new Moto(new Placa(placa), new Posicao(tipoPosicao)); patio.AdicionarMoto(moto);

            await _motoRepository.AdicionarAsync(moto);
            await _patioRepository.AtualizarAsync(patio);

            return new MotoDto(moto.Id, moto.Placa.Numero, moto.Status, moto.Posicao.ToString());
        }

        public async Task<MotoDto?> AlterarStatusMotoAsync(string placa, string novoStatus)
        {
            var moto = await _motoRepository.ObterPorPlacaAsync(placa);
            if (moto == null) return null;

            moto.AlterarStatus(novoStatus);
            await _motoRepository.AtualizarAsync(moto);

            return new MotoDto(moto.Id, moto.Placa.Numero, moto.Status, moto.Posicao.ToString());
        }

        public async Task<PatioDto?> ObterPatioComMotosAsync(Guid patioId)
        {
            var patio = await _patioRepository.ObterPorIdAsync(patioId);
            if (patio == null) return null;

            var motosDto = patio.Motos
                .Select(m => new MotoDto(m.Id, m.Placa.Numero, m.Status, m.Posicao.ToString()))
                .ToList();

            return new PatioDto(patio.Id, patio.Nome, motosDto);
        }

        public async Task RemoverMotoAsync(Guid patioId, string placa)
        {
            var patio = await _patioRepository.ObterPorIdAsync(patioId);
            if (patio == null) throw new Exception("Pátio não encontrado");

            var moto = patio.Motos.FirstOrDefault(m => m.Placa.Numero == placa);
            if (moto == null) throw new Exception("Moto não encontrada");

            patio.RemoverMoto(moto);
            await _motoRepository.RemoverAsync(moto);
            await _patioRepository.AtualizarAsync(patio);
        }
        public async Task<MotoDto?> ObterMotoPorPlacaAsync(string placa)
        {
            var moto = await _motoRepository.ObterPorPlacaAsync(placa);
            if (moto == null) return null;
            return new MotoDto(moto.Id, moto.Placa.Numero, moto.Status, moto.Posicao.ToString());
        }

    }
}
