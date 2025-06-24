namespace Poliedro.Billing.Domain.PrepareInvoicesBilling.Ports;

public interface IPrepareInvoicesBillingFactory
{
 IPrepareInvoicesBillingStrategy GetProcessor(string clientType);
}