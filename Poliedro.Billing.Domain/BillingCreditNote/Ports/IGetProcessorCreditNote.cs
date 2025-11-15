using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Domain.BillingCreditNote.Ports;
public interface IGetProcessorCreditNote
{
    Task<ICreateCreditNote> GetProcessorAsync(ResolutionType resolutyonType, string provider);
}
