using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;
namespace Poliedro.Billing.Application.BillingCreditNote.Services.Factories.Plemsi;
public interface IBillingCreditNoteSenderFactory
{
    IBillingCreditNoteSender ResolveCreditNote(string provider, ResolutionType typeResolution);
}
