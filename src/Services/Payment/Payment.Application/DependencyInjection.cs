using Microsoft.Extensions.DependencyInjection;

using Payment.Application.UseCases;

namespace Payment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProcessarPagamentoUseCase, ProcessarPagamentoUseCase>();
        return services;
    }
}
