using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Infrastructure.Consumers;

namespace Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Registra os consumidores que moram nesta assembly
            x.AddConsumer<PedidoCriadoEventConsumer>();
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

                // Aqui é o Pulo do Gato para Consumers:
                // Dizemos ao RabbitMQ para criar uma fila "pedido-criado-queue" e atrelar ao nosso Consumer
                cfg.ReceiveEndpoint("payment-pedido-criado-queue", e =>
                {
                    e.ConfigureConsumer<PedidoCriadoEventConsumer>(context);
                });

                cfg.ReceiveEndpoint("payment-estoque-recusado-queue", e =>
                {
                    e.ConfigureConsumer<EstoqueRecusadoEventConsumer>(context);
                });
            });
        });

        return services;
    }
}
