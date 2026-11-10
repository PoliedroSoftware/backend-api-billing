using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
using Poliedro.Billing.Infraestructure.External.Provider.Factory;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi.Senders;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.Siigo;
using Poliedro.Billing.Infraestructure.External.Provider.Provider.TNS;

namespace Poliedro.Billing.Infraestructure.External.Provider
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddBillingProviders(
            this IServiceCollection services)
        {
            // Factory que resuelve qué provider usar
            services.AddScoped<IBillingProviderFactory, BillingProviderFactory>();

            // Dependencias específicas de Plemsi FE
            services.AddScoped<PrepareBillingFE>();

            // Senders específicos de proveedores
            services.AddScoped<ISenderPlemsi, PlemsiFESender>();

            // Providers concretos
            services.AddScoped<IBillingProvider, PlemsiBillingFEProvider>();
            services.AddScoped<IBillingProvider, SiigoBillingFEProvider>();
            services.AddScoped<IBillingProvider, TNSBillingFEProvider>();

            return services;
        }
    }
}
