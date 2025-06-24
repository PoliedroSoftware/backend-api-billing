namespace Poliedro.Billing.Domain.PrepareInvoicesBilling.Ports;

public interface  IPrepareInvoicesBillingStrategy
{
   Task<List<object>> PrepareInvoicesAsync(List<object> invoices, CancellationToken cancellationToken);
}