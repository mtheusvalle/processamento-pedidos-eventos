using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Data.Context;
using Payment.Data.Repositories;
using Payment.Domain.Interfaces;

namespace Payment.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PaymentDb")));

        services.AddScoped<IPagamentoRepository, PagamentoRepository>();

        return services;
    }
}
