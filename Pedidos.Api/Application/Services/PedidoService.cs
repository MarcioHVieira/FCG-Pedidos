using Fcg.Common.Enums;
using Fcg.Common.Extensions;
using Fcg.Common.Middleware.Exceptions;
using Pedidos.Api.Application.Constants;
using Pedidos.Api.Application.DTOs;
using Pedidos.Api.Application.Mappers;
using Pedidos.Api.Domain.Entities;
using Pedidos.Api.Domain.Interfaces;
using Pedidos.Api.Infrastructure.Search.Services;

namespace Pedidos.Api.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ISearchService _searchService;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(IPedidoRepository pedidoRepository, 
                             ISearchService searchService, 
                             ILogger<PedidoService> logger)
        {
            _pedidoRepository = pedidoRepository;
            _searchService = searchService;
            _logger = logger;
        }

        public async Task<PedidoResponseDto> ObterPedido(Guid pedidoId, Guid usuarioId)
        {
            var pedido = await _pedidoRepository.ObterPorId(pedidoId);

            if (pedido == null || (usuarioId != Guid.Empty && pedido.UsuarioId != usuarioId))
                throw new KeyNotFoundException("Pedido não encontrado com o Id informado");

            return pedido.ToDto();
        }

        public async Task<IEnumerable<PedidoResponseDto>> ObterPedidos()
        {
            var pedidos = await _pedidoRepository.ObterTodos();

            return pedidos.Select(p => p.ToDto());
        }

        public async Task<IEnumerable<PedidoResponseDto>> ObterPedidosAtivos(Guid usuarioId)
        {
            var pedidos = await _pedidoRepository.ObterTodosPorUsuario(usuarioId);

            return pedidos.Select(p => p.ToDto());
        }

        public async Task AdicionarPedido(PedidoAdicionarDto pedidoDto)
        {
            var pedido = pedidoDto.ToDomain();
            await ProcessarPedido(pedido, _pedidoRepository.Adicionar);
            //await _searchService.AtualizarPopularidadeAsync(pedido.JogoId);
        }

        public async Task AlterarPedido(PedidoAlterarDto pedidoDto)
        {
            await ProcessarPedido(pedidoDto.ToDomain(), pedido => _pedidoRepository.Alterar(pedido));
        }

        public async Task AtivarPedido(Guid pedidoId)
        {
            await _pedidoRepository.Ativar(pedidoId);
        }

        public async Task DesativarPedido(Guid pedidoId)
        {
            await _pedidoRepository.Desativar(pedidoId);
        }

        public async Task AtualizarStatusAsync(Guid pedidoId, StatusPedido status)
        {
            var pedido = await _pedidoRepository.ObterPorId(pedidoId);

            if (pedido == null)
            {
                _logger.LogService(
                    ServiceConstants.ServiceName,
                    "AtualizarStatusAsync",
                    "Aviso",
                    "Id do pedido não foi encontrado",
                    new { Status = status });

                return;
            }

            pedido.AlterarStatus(status);
            await _pedidoRepository.Alterar(pedido);
        }

        #region Métodos Privados
        private async Task ProcessarPedido(Pedido pedido, Func<Pedido, Task> operacao)
        {
            if (await _pedidoRepository.Existe(pedido))
                throw new ConflitoException("Já existe um pedido com as mesmas informações.");

            await operacao(pedido);
        }
        #endregion
    }
}
