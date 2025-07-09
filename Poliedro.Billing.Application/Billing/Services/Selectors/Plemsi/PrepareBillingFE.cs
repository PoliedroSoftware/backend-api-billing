using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingFE(IUdateItem prepareItemElectronic) : ICreateBillingStrategy{

   

    public Task<IEnumerable<CreateBilling>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken)
    {
        var Prepared = invoices.Select(invoice =>
        {
            if (invoice.ItemElectronicEntity != null)
            {
                ////invoice.ItemElectronicEntity = prepareItemElectronic.UpdateItemAsync(invoice.ItemElectronicEntity);

                //double totalToBase = invoice.ItemElectronicEntity.Sum(item => item.LineExtensionAmount);
                //double totalTaxableAmount = invoice.ItemElectronicEntity.Sum(item => item.TaxTotals.Sum(tax => tax.TaxAmount));
                //double totalToPay = totalToBase + totalTaxableAmount;

                //invoice.InvoiceBaseTotal = (decimal)totalToBase;
                //invoice.InvoiceTaxExclusiveTotal = (decimal)totalToBase;
                //invoice.InvoiceTaxInclusiveTotal = (decimal)totalToPay;
                //invoice.TotalToPay = (decimal)totalToPay;
                //invoice.FinalTotalToPay = (decimal)totalToPay;
            }


            //List<AllTaxTotalEntity> allTaxTotals = [];

            //invoice.AllTaxTotalEntity = GetAllTaxTotals();






            return invoice;
        });


        return Task.FromResult(Prepared);
    }
}