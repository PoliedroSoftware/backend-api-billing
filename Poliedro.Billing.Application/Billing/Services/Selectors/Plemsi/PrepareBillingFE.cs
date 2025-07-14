using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingFE(
    IPrepareItemBilling _prepareItemElectronic,
    IGetAllTaxTotalsBilling _getAllTaxTotals,
    IGetLastInvoiceBilling _getLastInvoiceBilling
    ) : ICreateBilling {



    public async Task<IEnumerable<CreateBilling>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken)
    {
        var preparedArray = await Task.WhenAll(
            invoices.Select(async invoice =>
        {
            if (invoice.ItemElectronicEntity != null)
            {
                invoice.ItemElectronicEntity = await _prepareItemElectronic.PrepareItemBillingAsync(invoice.ItemElectronicEntity);

                double totalToBase = invoice.ItemElectronicEntity.Sum(item => item.LineExtensionAmount);
                double totalTaxableAmount = invoice.ItemElectronicEntity.Sum(item => item.TaxTotals.Sum(tax => tax.TaxAmount));
                double totalToPay = totalToBase + totalTaxableAmount;

                invoice.InvoiceBaseTotal = (decimal)totalToBase;
                invoice.InvoiceTaxExclusiveTotal = (decimal)totalToBase;
                invoice.InvoiceTaxInclusiveTotal = (decimal)totalToPay;
                invoice.TotalToPay = (decimal)totalToPay;
                invoice.FinalTotalToPay = (decimal)totalToPay;

                List<AllTaxTotalEntity> AllTaxTotals = [];

                AllTaxTotals = await _getAllTaxTotals.IGetAllTaxTotalsBillingAsync(invoice.ItemElectronicEntity);

            }

            int InvoiceNumber = int.Parse(invoice.Number[^4..]);

            int Invoice = await _getLastInvoiceBilling.GetLastInvoiceNumberAsync(invoice.CustomerEntity, cancellationToken);


            Invoice = (Invoice <= 0 || Invoice < InvoiceNumber) ? InvoiceNumber : Invoice + 1;


            return invoice;
        })
        );
        return preparedArray;
        }
    }
