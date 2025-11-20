using Pedidos.Api.Infrastructure.Search.Models;
using Elastic.Clients.Elasticsearch;

namespace Pedidos.Api.Infrastructure.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly ElasticsearchClient _client;

        public SearchService(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task AtualizarPopularidadeAsync(Guid idJogo)
        {
            var response = await _client.GetAsync<JogoElastic>(idJogo.ToString());

            if (!response.Found || response.Source == null)
                return;

            response.Source.Popularidade += 1;

            await _client.IndexAsync(response.Source, i => i.Id(idJogo.ToString()));
        }

    }
}