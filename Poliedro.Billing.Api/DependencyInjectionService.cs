using Poliedro.Billing.Api.Common.Configurations;

namespace Poliedro.Billing.Api;

public static class DependencyInjectionService
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services.AddFluentValidationServices();
        return services;
    }
}

