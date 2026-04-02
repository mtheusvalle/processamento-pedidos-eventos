using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Inventory.Worker.Consumers;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    var configuration = hostContext.Configuration;

    services.AddMassTransit(x =>
    {
        x.AddConsumer<PedidoCriadoEventConsumer>();

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

            // Cada serviço tem sua própria fila ao consumir o mesmo evento para que haja pub/sub
            cfg.ReceiveEndpoint("inventory-pedido-criado-queue", e =>
            {
                e.ConfigureConsumer<PedidoCriadoEventConsumer>(context);
            });
        });
    });
});

var host = builder.Build();
await host.RunAsync();
