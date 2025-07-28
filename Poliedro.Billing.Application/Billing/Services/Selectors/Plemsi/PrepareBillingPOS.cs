using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;


namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingPOS : ICreateBilling //POS
{
    public Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, DateTime ExpirationDate, int FinalRange, string Prefix, string Apikey, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }


}