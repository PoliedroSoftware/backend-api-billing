using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Services.Preparation;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.Billing.Services.Factories;

public class BillingPrepareFactory(IServiceProvider _serviceProvider) : IGetProcessorBilling
{
    public Task<ICreateBilling> GetProcessorAsync(ResolutionType resolutionType)
    {
        // Preparation applies resolution/document rules. Provider wire-format
        // translation belongs to the selected infrastructure sender.
        return resolutionType switch
        {
            ResolutionType.FE =>
                Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingFE>()
                    as ICreateBilling),

            ResolutionType.POS =>
                Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingPOS>()
                    as ICreateBilling),

            _ => throw new ArgumentException(
                $"Unknown document type ({resolutionType})")
        };

    }
}
