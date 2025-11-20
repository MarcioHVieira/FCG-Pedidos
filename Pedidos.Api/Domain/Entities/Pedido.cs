using Fcg.Common.Entities;
using Fcg.Common.Enums;

namespace Pedidos.Api.Domain.Entities
{
    public class Pedido : EntityBase
    {
        public Guid UsuarioId { get; private set; }
        public string UsuarioNome { get; private set; }
        public Guid JogoId { get; private set; }
        public string JogoTitulo { get; private set; }
        public decimal Valor { get; private set; }
        public StatusPedido Status { get; private set; }

        //EF
        protected Pedido() { }

        private Pedido(Guid id, Guid usuarioId, string usuarioNome, Guid jogoId, string jogoTitulo, decimal valor, StatusPedido status)
        {
            Id = id;
            UsuarioId = usuarioId;
            UsuarioNome = usuarioNome;
            JogoId = jogoId;
            JogoTitulo = jogoTitulo;
            Valor = valor;
            Status = status;
        }

        public void AlterarStatus(StatusPedido status)
        {
            Status = status;
        }

        public static Pedido CriarAlterar(Guid? id, Guid usuarioId, string usuarioNome, Guid jogoId, string jogoTitulo, 
                                          decimal valor, StatusPedido status)
        {
            return new Pedido(id ?? Guid.NewGuid(), usuarioId, usuarioNome, jogoId, jogoTitulo, valor, status);
        }
    }
}
