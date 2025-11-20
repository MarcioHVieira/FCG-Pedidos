using Fcg.Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Domain.Entities;
using Pedidos.Api.Domain.Interfaces;

namespace Pedidos.Api.Infrastructure.Data
{
    public class PedidoRepository : RepositoryBase<Pedido, PedidosDbContext>, IPedidoRepository
    {
        private readonly PedidosDbContext _context;

        public PedidoRepository(PedidosDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<Pedido?> ObterPorId(Guid id)
        {
            return await _context.Pedidos.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> Existe(Pedido pedido)
        {
            return await _context.Pedidos
                .AnyAsync(p => p.Id != pedido.Id && p.UsuarioId == pedido.UsuarioId && p.JogoId == pedido.JogoId);
        }

        public override async Task<IEnumerable<Pedido>> ObterTodos()
        {
            return await _context.Pedidos.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> ObterTodosPorUsuario(Guid usuarioId)
        {
            return await _context.Pedidos.AsNoTracking()
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }
    }
}
