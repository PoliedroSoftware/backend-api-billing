using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Domain.Siigo.DomainServices;
using Poliedro.Billing.Infraestructure.External.Siigo.DomainServices;
using Poliedro.Billing.Infraestructure.External.Siigo.Services;
using Poliedro.Billing.Infraestructure.External.Siigo.Services.Imp;

namespace Poliedro.Billing.Infraestructure.External.Siigo
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddExternalSiigo(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISiigoDomainService, SiigoDomainService>();
            services.AddSingleton<ISettingsService, SettingsService>();

            return services;
        }
    }
}
