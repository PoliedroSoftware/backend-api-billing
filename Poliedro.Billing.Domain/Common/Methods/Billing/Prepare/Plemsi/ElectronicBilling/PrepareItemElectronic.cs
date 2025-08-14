using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;

public class PrepareItemElectronic : IPrepareItemBilling
{
       public async Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items)
        {
        List<ItemElectronicEntity> itemsInvoiceResponse = [];

        foreach (ItemElectronicEntity item in items)
        {
            var itemInvoice = new ItemElectronicEntity
            {
                UnitMeasureId = 70,
                LineExtensionAmount = item.UnitPrice * item.InvoicedQuantity,
                InvoicedQuantity = item.InvoicedQuantity,
                FreeOfChargeIndicator = false,


                AllowanceCharges = [],


                TaxTotals = [
                     new TaxTotalEntity {
                         TaxId = 1,
                         Percent = item.TaxTotals?.FirstOrDefault()?.Percent ?? 0,
                         TaxAmount = (item.TaxTotals?.FirstOrDefault()?.TaxAmount ?? 0) * item.InvoicedQuantity,
                         TaxableAmount = item.UnitPrice * item.InvoicedQuantity
                }],


                WithHoldingTaxTotal = [],


                Description = $"{item.Description} {(item.TaxTotals?.FirstOrDefault()?.TaxAmount > 0 ? $"IVA {item.TaxTotals?.FirstOrDefault()?.TaxAmount}" : "")}",
                Notes = "",
                Code = item.Code,
                TypeItemIdentificationId = 1,
                PriceAmount = item.UnitPrice,
                BaseQuantity = item.InvoicedQuantity,
            };
            itemsInvoiceResponse.Add(itemInvoice);

        }

        return await Task.FromResult(itemsInvoiceResponse);
    }
}
