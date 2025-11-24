using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
namespace Poliedro.Billing.Domain.BillingCreditNote.Ports;
public interface IUpdateCurrentlyNumberCreditNote
{
    Task UpdateCurrentlyNumberCreditNoteAsync(ParametersCurrentlyNumber Parameters, CancellationToken cancellationToken);
}
