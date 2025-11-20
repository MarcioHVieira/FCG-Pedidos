using Fcg.Common.Messaging.Abstractions;
using Fcg.Common.Messaging.RabbitMQ;
using Fcg.Common.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Domain.Events;
using Pedidos.Api.Infrastructure.Data;
using Pedidos.Api.Infrastructure.Search;
using Prometheus;
using System.Text.Json.Serialization;

namespace Pedidos.Api.Configurations
{
    public static class ApiConfig
    {
        public static WebApplicationBuilder AddApiConfiguration(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
                builder.Configuration.AddUserSecrets<Program>();

            builder.Logging.ClearProviders();
            builder.Logging.AddApplicationInsights(
                configureTelemetryConfiguration: (config) =>
                    config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
                configureApplicationInsightsLoggerOptions: (options) => { }
            );

            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            builder.Services.AddDbContext<PedidosDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddElasticsearchClient(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthorization();
            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());

            return builder;
        }

        public static WebApplication UseApiConfiguration(this WebApplication app)
        {
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.UseMiddleware<TratamentoErrosMiddleware>();
            app.UseAuthorization();
            app.MapControllers();

            var configuration = app.Services.GetRequiredService<IConfiguration>();
            var provider = configuration["Messaging:Provider"];

            if (provider == "RabbitMQ")
            {
                var consumer = app.Services.GetRequiredService<RabbitMqEventConsumer<PagamentoRealizadoEvent>>();
                consumer.Start();
            }
            else if (provider == "ServiceBus")
            {
                var consumer = app.Services.GetRequiredService<IEventConsumer<PagamentoRealizadoEvent>>();
                consumer.Start();
            }

            return app;
        }
    }
}
