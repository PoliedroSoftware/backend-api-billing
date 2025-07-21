using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;


namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingPOS : ICreateBilling //POS
{
    

    public Task<IEnumerable<object>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, DateTime ExpirationDate, int FinalRange, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}