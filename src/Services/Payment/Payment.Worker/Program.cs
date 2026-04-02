using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application;
using Payment.Data;
using Payment.Infrastructure;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    // Mais uma vez, pura Arquitetura Limpa:
    services.AddApplication();
    services.AddData(hostContext.Configuration);
    services.AddInfrastructure(hostContext.Configuration);
});

var host = builder.Build();
// O padrão Worker/Host do .NET 10 já sobe os HostedServices (incluindo o bus do MassTransit) automaticamente
await host.RunAsync();
