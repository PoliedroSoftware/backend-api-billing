using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Infrastructure.Providers.Factory;
using Poliedro.Billing.Infrastructure.Providers.Providers.Plemsi;
using Poliedro.Billing.Infrastructure.Providers.Providers.Siigo;
using Poliedro.Billing.Infrastructure.Providers.Providers.TNS;

namespace Poliedro.Billing.Infrastructure.Providers;

public static  class DependencyInjectionService
{
    public static IServiceCollection AddBillingProviders(this IServiceCollection services)
    {
        services.AddScoped<IBillingProviderFactory, BillingProviderFactory>();
        services.AddScoped<IBillingProvider, PlemsiBillingProvider>();
        services.AddScoped<IBillingProvider, SiigoBillingProvider>();
        services.AddScoped<IBillingProvider, TNSBillingProvider>();


        return services;
    }
}
