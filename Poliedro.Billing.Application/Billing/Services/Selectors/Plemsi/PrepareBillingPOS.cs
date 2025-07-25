using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;


namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingPOS : ICreateBilling //POS
{
    

    Task<IEnumerable<(CreateBilling Billing, object Output)>> ICreateBilling.CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, DateTime ExpirationDate, int FinalRange, string Prefix, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}