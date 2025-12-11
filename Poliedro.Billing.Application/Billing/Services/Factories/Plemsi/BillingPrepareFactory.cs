using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
using Poliedro.Billing.Application.Billing.Services.Selectors.Siigo;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;

public class BillingPrepareFactory(IServiceProvider _serviceProvider) : IGetProcessorBilling
{
    public Task<ICreateBilling> GetProcessorAsync(ResolutionType resolutionType, string provider)
    {
        return (provider, resolutionType.ToString()) switch
        {
            ("PLEMSI", "FE") => Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingFE>()
                        as ICreateBilling),

            ("PLEMSI", "POS") => Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingPOS>()
                        as ICreateBilling),

            ("SIIGO", "FE") => Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingSiigoFE>()
                        as ICreateBilling),

            ("SIIGO", "POS") => Task.FromResult(
                _serviceProvider.GetRequiredService<PrepareBillingSiigoPOS>()
                        as ICreateBilling),


            _ => throw new ArgumentException($"Unknown provider ({provider}) or type ({resolutionType})")
        };

    }
}