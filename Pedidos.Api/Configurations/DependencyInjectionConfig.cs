using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Pedidos.Api.Application.Services;
using Pedidos.Api.Domain.Interfaces;
using Pedidos.Api.Infrastructure.Data;
using Pedidos.Api.Infrastructure.Search.Services;

namespace Pedidos.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static WebApplicationBuilder RegisterDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
            builder.Services.AddScoped<IPedidoService, PedidoService>();
            builder.Services.AddScoped<ISearchService, SearchService>();
            builder.Services.AddScoped<PedidosDbContext>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<ElasticsearchClient>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var uri = configuration["Elasticsearch:Uri"];
                var useCloud = configuration.GetValue<bool>("Elasticsearch:UseCloud");
                var settings = new ElasticsearchClientSettings(new Uri(uri));

                if (useCloud)
                {
                    var username = configuration["Elasticsearch:Username"];
                    var password = configuration["Elasticsearch:Password"];
                    settings = settings.Authentication(new BasicAuthentication(username, password));
                }

                return new ElasticsearchClient(settings);
            });

            var messagingProvider = builder.Configuration["Messaging:Provider"];
            MessagingDependencyInjection.RegisterConsumers(builder, messagingProvider);

            return builder;
        }
    }
}
