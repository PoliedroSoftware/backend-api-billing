using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare;

public class PrepareItemElectronic : IUdateItem
{
       public async Task<List<ItemElectronicEntity>> UpdateItemAsync(List<ItemElectronicEntity> items)
    {
        List<ItemElectronicEntity> itemsInvoiceResponse = [];

        foreach (ItemElectronicEntity item in items)
        {
            var itemInvoice = new ItemElectronicEntity
            {
                UnitMeasureId = 70,
                LineExtensionAmount = item.PriceAmount * item.InvoicedQuantity,
                InvoicedQuantity = item.InvoicedQuantity,
                FreeOfChargeIndicator = false,


                AllowanceCharges = [],


                TaxTotals = [
                     new TaxTotalEntity {
                         TaxId = 1,
                         Percent = item.TaxTotals?.FirstOrDefault()?.Percent ?? 0,
                         TaxAmount = (item.TaxTotals?.FirstOrDefault()?.TaxAmount ?? 0) * item.InvoicedQuantity,
                         TaxableAmount = item.PriceAmount * item.InvoicedQuantity
                }],


                WithHoldingTaxTotal = [],


                Description = $"{item.Description} {(item.TaxTotals?.FirstOrDefault()?.TaxAmount > 0 ? $"IVA {item.TaxTotals?.FirstOrDefault()?.TaxAmount}" : "")}",
                Notes = "",
                Code = item.Code.ToString(),
                TypeItemIdentificationId = 1,
                PriceAmount = item.PriceAmount,
                BaseQuantity = item.InvoicedQuantity,
            };
            itemsInvoiceResponse.Add(itemInvoice);

        }

        return itemsInvoiceResponse;
    }
}
