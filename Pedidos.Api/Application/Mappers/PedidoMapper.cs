using Fcg.Common.Enums;
using Pedidos.Api.Application.DTOs;
using Pedidos.Api.Domain.Entities;

namespace Pedidos.Api.Application.Mappers
{
    public static class PedidoMapper
    {
        public static Pedido ToDomain(this PedidoAdicionarDto pedidoDto)
        {
            return Pedido.CriarAlterar(null, pedidoDto.UsuarioId, pedidoDto.UsuarioNome, pedidoDto.JogoId, 
                                       pedidoDto.JogoTitulo, pedidoDto.Valor, StatusPedido.Pendente);
        }

        public static Pedido ToDomain(this PedidoAlterarDto pedidoDto)
        {
            return Pedido.CriarAlterar(pedidoDto.Id, pedidoDto.UsuarioId, pedidoDto.UsuarioNome, pedidoDto.JogoId, 
                                       pedidoDto.JogoTitulo, pedidoDto.Valor, pedidoDto.Status);
        }

        public static PedidoResponseDto ToDto(this Pedido pedido)
        {
            return new PedidoResponseDto{Id = pedido.Id, UsuarioId = pedido.UsuarioId, UsuarioNome = pedido.UsuarioNome, JogoId = pedido.JogoId, 
                                         JogoTitulo = pedido.JogoTitulo, Valor = pedido.Valor, Status = pedido.Status };
        }
    }
}
