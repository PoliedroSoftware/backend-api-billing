using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Prepare;

namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingFE(PrepareItemElectronic prepareItemElectronic) : ICreateBillingStrategy{

   

    public Task<IEnumerable<CreateBilling>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken)
    {
        var Prepared = invoices.Select(invoice =>
        {
            if (invoice.ItemElectronicEntity != null)
            {
                invoice.ItemElectronicEntity = prepareItemElectronic.UpdateItem(invoice.ItemElectronicEntity);

            }
            return invoice;
        });


        return Task.FromResult(Prepared);
    }
}