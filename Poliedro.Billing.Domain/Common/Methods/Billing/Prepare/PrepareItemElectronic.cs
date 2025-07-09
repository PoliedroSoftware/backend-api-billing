using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare;

public class PrepareItemElectronic : IUdateItem
{
       public async Task<List<ItemElectronicEntity>> UpdateItemAsync(List<ItemElectronicEntity> items)
    {
        List<ItemElectronicEntity> itemsInvoiceResponse = [];

        //foreach (ItemsInvoiceEntity item in items)
        //{
        //    var itemInvoice = new ItemElectronicEntity
        //    {
        //        UnitMeasureId = 70,
        //        LineExtensionAmount = item.unit_preci * item.invoiced_quantity,
        //        InvoicedQuantity = item.invoiced_quantity,
        //        FreeOfChargeIndicator = false,
        //        AllowanceCharges = [],

        //        TaxTotals = [
        //             new TaxTotalEntity {
        //                 TaxId = 1,
        //                 Percent = item.percent,
        //                 TaxAmount = item.tax_amount * item.invoiced_quantity,
        //                 TaxableAmount = item.unit_preci * item.invoiced_quantity
        //        }],
        //        WithHoldingTaxTotal = [],
        //        Description = $"{item.description} {(item.tax_amount > 0 ? $"IVA {item.tax_amount}" : "")}",

        //        Notes = "",
        //        Code = item.code.ToString(),
        //        TypeItemIdentificationId = 1,
        //        PriceAmount = item.unit_preci,
        //        BaseQuantity = item.invoiced_quantity,
        //    };
        //    itemsInvoiceResponse.Add(itemInvoice);

        //}

        return itemsInvoiceResponse;
    }
}
