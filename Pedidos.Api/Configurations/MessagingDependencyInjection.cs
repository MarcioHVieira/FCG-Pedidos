using Fcg.Common.Enums;
using Fcg.Common.Messaging.Abstractions;
using Fcg.Common.Messaging.RabbitMQ;
using Fcg.Common.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Pedidos.Api.Application.Services;
using Pedidos.Api.Domain.Events;

namespace Pedidos.Api.Configurations
{
    public static class MessagingDependencyInjection
    {
        public static void RegisterConsumers(WebApplicationBuilder builder, string messagingProvider)
        {
            if (messagingProvider == "RabbitMQ")
            {
                builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
                RegisterRabbitMqConsumers(builder.Services, builder.Configuration);
            }
            else if (messagingProvider == "ServiceBus")
            {
                builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBus"));
                RegisterServiceBusConsumers(builder.Services, builder.Configuration);
            }
        }

        #region RabbitMQ
        private static void RegisterRabbitMqConsumers(IServiceCollection services, IConfiguration configuration)
        {
            var eventQueueMap = configuration.GetSection("RabbitMQ:Queues").Get<Dictionary<string, string>>();

            var eventHandlers = new Dictionary<Type, Func<IServiceProvider, object, Task>>
            {
                { typeof(PagamentoRealizadoEvent), async (provider, evento) =>
                    {
                        var pedidoService = provider.GetRequiredService<IPedidoService>();
                        await pedidoService.AtualizarStatusAsync(((PagamentoRealizadoEvent)evento).PedidoId, StatusPedido.Pago);
                    }
                },
                // Adicionar outros eventos
            };

            foreach (var eventType in eventHandlers.Keys)
            {
                if (!eventQueueMap.ContainsKey(eventType.Name) || string.IsNullOrWhiteSpace(eventQueueMap[eventType.Name]))
                    throw new InvalidOperationException($"Fila não configurada para o evento {eventType.Name}.");
            }

            foreach (var kvp in eventHandlers)
            {
                var eventType = kvp.Key;
                var handler = kvp.Value;

                if (!eventQueueMap.TryGetValue(eventType.Name, out var queueName) || string.IsNullOrWhiteSpace(queueName))
                    throw new InvalidOperationException($"Fila não configurada para o evento {eventType.Name}.");

                var method = typeof(MessagingDependencyInjection).GetMethod(nameof(AddRabbitMqConsumer), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var genericMethod = method.MakeGenericMethod(eventType);

                try
                {
                    genericMethod.Invoke(null, new object[] { services, eventType.Name, handler });
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Erro ao registrar consumidor para evento {eventType.Name} na fila {queueName}: {ex.Message}", ex);
                }
            }
        }

        private static void AddRabbitMqConsumer<TEvent>(
            IServiceCollection services,
            string queueKey,
            Func<IServiceProvider, object, Task> handler)
        {
            services.AddSingleton<RabbitMqEventConsumer<TEvent>>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var settings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                var instance = Activator.CreateInstance(
                    typeof(RabbitMqEventConsumer<TEvent>),
                    scopeFactory,
                    queueKey,
                    (Func<IServiceProvider, TEvent, Task>)((provider, evento) => handler(provider, evento)),
                    settings
                ) as RabbitMqEventConsumer<TEvent>;

                if (instance is null)
                    throw new InvalidOperationException("Não foi possível criar a instância de RabbitMqEventConsumer.");

                return instance;
            });
        }
        #endregion

        #region ServiceBus
        private static void RegisterServiceBusConsumers(IServiceCollection services, IConfiguration configuration)
        {
            var eventQueueMap = configuration.GetSection("ServiceBus:Queues").Get<Dictionary<string, string>>();

            var eventHandlers = new Dictionary<Type, Func<IServiceProvider, object, Task>>
            {
                { typeof(PagamentoRealizadoEvent), async (provider, evento) =>
                    {
                        var pedidoService = provider.GetRequiredService<IPedidoService>();
                        await pedidoService.AtualizarStatusAsync(((PagamentoRealizadoEvent)evento).PedidoId, StatusPedido.Pago);
                    }
                },
                // Adicionar outros eventos
            };

            foreach (var eventType in eventHandlers.Keys)
            {
                if (!eventQueueMap.ContainsKey(eventType.Name) || string.IsNullOrWhiteSpace(eventQueueMap[eventType.Name]))
                {
                    throw new InvalidOperationException($"Fila não configurada para o evento {eventType.Name}.");
                }
            }

            foreach (var kvp in eventHandlers)
            {
                var eventType = kvp.Key;
                var handler = kvp.Value;
                var queueName = eventQueueMap[eventType.Name];

                var method = typeof(MessagingDependencyInjection).GetMethod(nameof(AddServiceBusConsumer), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var genericMethod = method.MakeGenericMethod(eventType);

                try
                {
                    genericMethod.Invoke(null, new object[] { services, eventType.Name, handler });
                }
                catch (Exception)
                {
                    throw new InvalidOperationException($"Erro ao registrar consumidor para evento {eventType.Name} na fila {queueName}");
                }
            }
        }

        private static void AddServiceBusConsumer<TEvent>(
            IServiceCollection services,
            string eventKey,
            Func<IServiceProvider, object, Task> handler)
        {
            services.AddSingleton<IEventConsumer<TEvent>>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var settings = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;

                var instance = Activator.CreateInstance(
                    typeof(ServiceBusEventConsumer<TEvent>),
                    scopeFactory,
                    eventKey,
                    (Func<IServiceProvider, TEvent, Task>)((provider, evento) => handler(provider, evento)),
                    settings,
                    false
                ) as IEventConsumer<TEvent>;

                if (instance is null)
                    throw new InvalidOperationException("Não foi possível criar a instância de ServiceBusEventConsumer.");

                return instance;
            });
        }
        #endregion
    }
}
