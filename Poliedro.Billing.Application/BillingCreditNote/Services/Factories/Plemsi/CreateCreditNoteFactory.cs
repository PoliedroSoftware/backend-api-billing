
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.BillingCreditNote.Services.Selectors.Plemsi;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.BillingCreditNote.Services.Factories.Plemsi;
public class CreateCreditNoteFactory(IServiceProvider _serviceProvider) : IGetProcessorCreditNote
{
    public Task<ICreateCreditNote> GetProcessorAsync(ResolutionType resolutionType, string provider)
    {
        return (provider, resolutionType.ToString()) switch
        {
            ("PLEMSI", "FE") => Task.FromResult(
                _serviceProvider.GetRequiredService<PrepareCreditNoteBillingFE>()
                as ICreateCreditNote),
            ("PLEMSI", "POS") => Task.FromResult(
                _serviceProvider.GetRequiredService<PrepareCreditNoteBillingPOS>()
                as ICreateCreditNote),

           _ => throw new ArgumentException($"Unknown provider ({provider}) or type ({resolutionType})")

        };
    }
}
