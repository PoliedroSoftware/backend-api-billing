using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.DomainService.Imp;

public  class CreateBillingPOSRepository : ICreateBillingStrategy
{
    public Task<List<CreateBilling>> CreateInvoicesAsync(List<CreateBilling> invoices, CancellationToken cancellationToken)
    {

        return Task.FromResult(invoices);
    }
}