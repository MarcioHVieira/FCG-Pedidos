using Elastic.Clients.Elasticsearch;

namespace Pedidos.Api.Infrastructure.Search
{
    public static class ElasticsearchClientConfigExtensions
    {
        public static IServiceCollection AddElasticsearchClient(this IServiceCollection services, IConfiguration configuration)
        {
            var uri = configuration["Elasticsearch:Uri"];
            var defaultIndex = configuration["Elasticsearch:DefaultIndex"];

            var settings = new ElasticsearchClientSettings(new Uri(uri))
                .DefaultIndex(defaultIndex);

            var client = new ElasticsearchClient(settings);

            services.AddSingleton(client);

            return services;
        }
    }
}