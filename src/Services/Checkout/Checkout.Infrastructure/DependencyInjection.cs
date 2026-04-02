using Checkout.Domain.Interfaces;
using Checkout.Infrastructure.Services;
using Checkout.Infrastructure.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Respeitamos a Inversão de Controle: Quem pedir IMessageBusService no domínio, 
        // receberá do contêiner o MassTransitMessageBusService!
        services.AddScoped<IMessageBusService, MassTransitMessageBusService>();

        // Toda a sugeira do MassTransit fica isolada aqui!
        services.AddMassTransit(x =>
        {
            x.AddConsumer<PagamentoAprovadoEventConsumer>();
            x.AddConsumer<PagamentoRecusadoEventConsumer>();
            x.AddConsumer<EstoqueRecusadoEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var hostName = configuration["RabbitMQ:HostName"] ?? "localhost";
                var userName = configuration["RabbitMQ:UserName"] ?? "guest";
                var password = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(hostName, "/", h =>
                {
                    h.Username(userName);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
