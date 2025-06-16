using Microsoft.OpenApi.Models;
using Poliedro.Billing.Api.Common.Configurations;

namespace Poliedro.Billing.Api;

public static class DependencyInjectionService
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {

        services.AddFluentValidationServices();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Poliedro Billing API",
                Description = "Adminitracion de APIs para Billing Electronic"
            });

            options.EnableAnnotations();
        });
        return services;
    }
}

