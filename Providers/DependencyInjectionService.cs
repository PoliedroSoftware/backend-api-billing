using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Infraestructure.External.Provider.Factory;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.Siigo;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.TNS;

namespace Poliedro.Billing.Infraestructure.External.Provider
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddBillingProviders(this IServiceCollection services)
        {

            services.AddScoped<IBillingProviderFactory, BillingProviderFactory>();
            services.AddScoped<IBillingProvider, PlemsiBillingFEProvider>();
            services.AddScoped<IBillingProvider, SiigoBillingFEProvider>();
            services.AddScoped<IBillingProvider, TNSBillingFEProvider>();



            return services;
        }
    }
}
