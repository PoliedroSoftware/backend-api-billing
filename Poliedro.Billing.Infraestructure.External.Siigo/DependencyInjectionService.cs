using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Siigo.Ports;
using Poliedro.Billing.Infraestructure.External.Siigo.DomainServices;
using Poliedro.Billing.Infraestructure.External.Siigo.Services;
using Poliedro.Billing.Infraestructure.External.Siigo.Services.Imp;

namespace Poliedro.Billing.Infraestructure.External.Siigo
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddExternalSiigo(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISiigoGateway, SiigoDomainService>();
            services.AddSingleton<ISettingsService, SettingsService>();

            return services;
        }
    }
}
