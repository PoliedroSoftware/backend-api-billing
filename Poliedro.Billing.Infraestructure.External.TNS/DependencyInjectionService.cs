using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Domain.TNS.DomainServices;
using Poliedro.Billing.Infraestructure.External.TNS.DomainServices;
using Poliedro.Billing.Infraestructure.External.TNS.Services.Imp;
using Poliedro.Billing.Infraestructure.External.TNS.Services;

namespace Poliedro.Billing.Infraestructure.External.TNS
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddExternalTns(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITNSDomainService, TNSDomainService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            return services;
        }
    }
}
