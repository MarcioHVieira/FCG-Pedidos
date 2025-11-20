namespace Pedidos.Api.Infrastructure.Search.Services
{
    public interface ISearchService
    {
        Task AtualizarPopularidadeAsync(Guid idJogo);
    }
}
