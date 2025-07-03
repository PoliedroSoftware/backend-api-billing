
using Poliedro.Billing.Domain.BillingPos;

namespace Poliedro.Billing.Domain.Billing.Ports
{
    public interface IBillingSender
    {
        Task SendInvoicesAsync(IEnumerable<CreateBilling> Invoices, CancellationToken cancellationToken);
    }
}
