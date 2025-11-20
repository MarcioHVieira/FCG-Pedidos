using Fcg.Common.Enums;
using Pedidos.Api.Application.DTOs;

namespace Pedidos.Api.Application.Services
{
    public interface IPedidoService
    {
        Task<PedidoResponseDto> ObterPedido(Guid pedidoId, Guid usuarioId);
        Task<IEnumerable<PedidoResponseDto>> ObterPedidos();
        Task<IEnumerable<PedidoResponseDto>> ObterPedidosAtivos(Guid usuarioId);
        Task AdicionarPedido(PedidoAdicionarDto pedidoDto);
        Task AlterarPedido(PedidoAlterarDto pedidoDto);
        Task AtivarPedido(Guid pedidoId);
        Task DesativarPedido(Guid pedidoId);
        Task AtualizarStatusAsync(Guid pedidoId, StatusPedido status);
    }
}
