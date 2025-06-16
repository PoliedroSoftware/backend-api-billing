using FluentValidation.AspNetCore;
using FluentValidation;

namespace Poliedro.Billing.Api.Controllers.v1.FERetail;

public static class FluentValidationFERetail
{
    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }

        return services;
    }
}






