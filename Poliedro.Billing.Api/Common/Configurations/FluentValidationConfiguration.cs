using FluentValidation;

namespace Poliedro.Billing.Api.Common.Configurations;

public static class FluentValidationConfiguration
{
    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        // Validators live in Application. Scanning every loaded assembly can pull in
        // provider/runtime assemblies that are unrelated and may not be compatible.
        services.AddValidatorsFromAssembly(typeof(Poliedro.Billing.Application.Billing.Commands.CreateBilling.CreateBillingValidator).Assembly);
        return services;
    }
}
