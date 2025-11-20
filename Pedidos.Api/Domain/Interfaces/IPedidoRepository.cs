using Pedidos.Api.Domain.Entities;

namespace Pedidos.Api.Domain.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObterPorId(Guid id);
        Task<IEnumerable<Pedido>> ObterTodos();
        Task<IEnumerable<Pedido>> ObterTodosAtivos();
        Task Adicionar(Pedido pedido);
        Task Alterar(Pedido pedido, bool AlterarAtivo = false);
        Task Remover(Guid id);
        Task Ativar(Guid id);
        Task Desativar(Guid id);

        Task<bool> Existe(Pedido pedido);
        Task<IEnumerable<Pedido>> ObterTodosPorUsuario(Guid usuarioId);
    }
}
