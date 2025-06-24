using Poliedro.Billing.Domain.PrepareInvoicesBilling.Ports;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.PrepareInvoicesBilling.DomainService.Impl;

public class PrepareInvoicesBillingFERepository : IPrepareInvoicesBillingStrategy
{
    public  Task<List<object>> PrepareInvoicesAsync(List<object> invoices, CancellationToken cancellationToken)
    {

        return Task.FromResult(invoices);
    }
}