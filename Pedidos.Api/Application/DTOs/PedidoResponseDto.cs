using Fcg.Common.Enums;

namespace Pedidos.Api.Application.DTOs
{
    public class PedidoResponseDto
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public Guid JogoId { get; set; }
        public string JogoTitulo { get; set; }
        public decimal Valor { get; set; }
        public StatusPedido Status { get; set; }
    }
}
