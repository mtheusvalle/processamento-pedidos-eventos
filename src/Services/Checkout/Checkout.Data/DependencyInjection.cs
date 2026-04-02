using Checkout.Data.Context;
using Checkout.Data.Repositories;
using Checkout.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuramos o DbContext para ligar no PostgreSQL puxando a String de Conexão.
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("CheckoutDb")));

        // 2. Injetamos nossa Implementação de Repositório resolvendo a interface.
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}
