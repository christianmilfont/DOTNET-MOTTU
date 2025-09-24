using Mottu_DOTNET.src.Application.DTOs;
using Mottu_DOTNET.src.Application.Interfaces;
using Mottu_DOTNET.src.Domain.Entities;
using Mottu_DOTNET.src.Domain.ValueObjects;
using Mottu_DOTNET.src.Infrastructure.Repositories;

namespace Mottu_DOTNET.src.Application.Services
{
    public class PatioService
    {
        private readonly IPatioRepository _patioRepository;
        private readonly IMotoRepository _motoRepository;
        private readonly IClienteRepository _clienteRepository;

        public PatioService(IPatioRepository patioRepository, IMotoRepository motoRepository, IClienteRepository clienteRepository)
        {
            _patioRepository = patioRepository;
            _motoRepository = motoRepository;
            _clienteRepository = clienteRepository;

        }

        public async Task<PatioDto> CriarPatioAsync(string nome)
        {
            var patio = new Patio(nome);
            await _patioRepository.AdicionarAsync(patio);
            return new PatioDto(patio.Id, patio.Nome, new List<MotoDto>());
        }
        public async Task<bool> RemoverPatioAsync(Guid id)
        {
            var patio = await _patioRepository.ObterPorIdAsync(id);
            if (patio == null) return false;

            await _patioRepository.RemoverAsync(patio);
            return true;
        }


        public async Task<MotoDto> AdicionarMotoAsync(Guid patioId, string placa, string posicao, Guid? clienteId = null)
        {
            var patio = await _patioRepository.ObterPorIdAsync(patioId);
            if (patio == null) throw new Exception("Pátio não encontrado");

            if (!Enum.TryParse<Posicao.TipoPosicao>(posicao, ignoreCase: true, out var tipoPosicao))
                throw new Exception("Tipo de posição inválido");

            var moto = new Moto(new Placa(placa), new Posicao(tipoPosicao));

            // Se clienteId for informado, associar a moto ao cliente
            if (clienteId.HasValue)
            {
                // Aqui você precisaria obter o cliente do repositório e associar a moto
                var cliente = await _clienteRepository.ObterPorIdAsync(clienteId.Value);
                if (cliente == null) throw new Exception("Cliente não encontrado");
                moto.AssociarCliente(cliente);
            }

            patio.AdicionarMoto(moto);

            await _motoRepository.AdicionarAsync(moto);
            await _patioRepository.AtualizarAsync(patio);

            return new MotoDto(moto.Id, moto.Placa.Numero, moto.Status, moto.Posicao.ToString(), moto.ClienteId);
        }

        public async Task<MotoDto?> AlterarStatusMotoAsync(string placa, string novoStatus)
        {
            var moto = await _motoRepository.ObterPorPlacaAsync(placa);
            if (moto == null) return null;

            moto.AlterarStatus(novoStatus);
            await _motoRepository.AtualizarAsync(moto);

            return new MotoDto(moto.Id, moto.Placa.Numero, moto.Status, moto.Posicao.ToString(), moto.ClienteId);
        }

        public async Task<PatioDto?> ObterPatioComMotosAsync(Guid patioId)
        {
            var patio = await _patioRepository.ObterPorIdAsync(patioId);
            if (patio == null) return null;

            var motosDto = patio.Motos
                .Select(m => new MotoDto(m.Id, m.Placa.Numero, m.Status, m.Posicao.ToString(), m.ClienteId))
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
        public async Task<MotoDto?> AtualizarMotoAsync(string placa, string status, string posicao)
        {
            var moto = await _motoRepository.ObterPorPlacaAsync(placa);
            if (moto == null) return null;

            moto.AlterarStatus(status);

            if (Enum.TryParse<Posicao.TipoPosicao>(posicao, ignoreCase: true, out var tipoPosicao))
            {
                moto.AlterarPosicao(new Posicao(tipoPosicao));
            }

            await _motoRepository.AtualizarAsync(moto);

            return new MotoDto(moto.Id, moto.Placa.Numero, moto.Status, moto.Posicao.ToString(), moto.ClienteId);
        }
        public async Task<MotoDto?> ObterMotoPorPlacaAsync(string placa)
        {
            var moto = await _motoRepository.ObterPorPlacaAsync(placa);
            if (moto == null) return null;
            return new MotoDto(moto.Id, moto.Placa.Numero, moto.Status, moto.Posicao.ToString(), moto.ClienteId);
        }
        public async Task<List<PatioDto>> ObterPatiosPaginadosAsync(int pageNumber, int pageSize)
        {
            var patios = await _patioRepository.ObterPaginadoAsync(pageNumber, pageSize);
            return patios.Select(p => new PatioDto(p.Id, p.Nome, p.Motos.Select(m => new MotoDto(m.Id, m.Placa.Numero, m.Status, m.Posicao.ToString(), m.ClienteId)).ToList())).ToList();
        }

        public async Task<int> ObterTotalPatiosAsync()
        {
            return await _patioRepository.ObterTotalAsync();
        }

        public async Task<List<MotoDto>> ObterMotosPaginadasAsync(int pageNumber, int pageSize)
        {
            var motos = await _motoRepository.ObterPaginadoAsync(pageNumber, pageSize);
            return motos.Select(m => new MotoDto(m.Id, m.Placa.Numero, m.Status, m.Posicao.ToString(), m.ClienteId)).ToList();
        }

        public async Task<int> ObterTotalMotosAsync()
        {
            return await _motoRepository.ObterTotalAsync();
        }
       

    }
}
