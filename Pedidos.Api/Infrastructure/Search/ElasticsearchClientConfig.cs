using Elastic.Clients.Elasticsearch;

namespace Pedidos.Api.Infrastructure.Search
{
    public class ElasticsearchClientConfig
    {
        private readonly IConfiguration _configuration;

        public ElasticsearchClientConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ElasticsearchClient CreateClient()
        {
            var uri = _configuration["Elasticsearch:Uri"];
            var defaultIndex = _configuration["Elasticsearch:DefaultIndex"];

            var settings = new ElasticsearchClientSettings(new Uri(uri))
                .DefaultIndex(defaultIndex);

            return new ElasticsearchClient(settings);
        }
    }
}