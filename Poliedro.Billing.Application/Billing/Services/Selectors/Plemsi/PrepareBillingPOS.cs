using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingPOS : ICreateBilling <CreateBilling, FERetailelectronicEntity> //POS
{
    public async Task<IEnumerable<FERetailelectronicEntity>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken)
    {

        var results = new List<FERetailelectronicEntity>(); //POS

        return results;
    }
}