using Checkout.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra todos os Casos de Uso
        services.AddScoped<ICriarPedidoUseCase, CriarPedidoUseCase>();
        
        return services;
    }
}
