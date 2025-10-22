using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
using System;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;

public class PrepareItemElectronic : IPrepareItemBilling
{
    public async Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items)
    {
        List<ItemElectronicEntity> itemsInvoiceResponse = [];

        foreach (ItemElectronicEntity item in items)
        {
            
            double subtotal = item.UnitPrice * item.InvoicedQuantity;

            
            double discount = (double)(item.LineDiscountAmount > 0 ? item.LineDiscountAmount : 0);
            double baseAfterDiscount = subtotal - discount;

            
            double taxAmount = baseAfterDiscount * (item.Percent / 100);

            
            double total = baseAfterDiscount + taxAmount;

            
            var itemInvoice = new ItemElectronicEntity
            {
                UnitMeasureId = 70,
                LineExtensionAmount = baseAfterDiscount, 
                InvoicedQuantity = item.InvoicedQuantity,
                FreeOfChargeIndicator = false,
                AllowanceCharges = [],

                TaxTotals = [
                    new TaxTotalEntity {
                        TaxId = 1,
                        Percent = item.Percent,
                        TaxAmount = taxAmount,
                        TaxableAmount = baseAfterDiscount
                    }
                ],

                WithHoldingTaxTotal = [],

                Description = $"{item.Description} {(taxAmount > 0 ? $"IVA {taxAmount}" : "")}",
                Notes = "",
                Code = item.Code,
                TypeItemIdentificationId = 1,
                PriceAmount = item.UnitPrice,
                BaseQuantity = item.InvoicedQuantity,

                UnitPriceBeforeDiscount = item.UnitPriceBeforeDiscount,
                LineDiscountAmount = discount,          
                LineDiscountType = item.LineDiscountType
            };

            itemsInvoiceResponse.Add(itemInvoice);
        }

        return await Task.FromResult(itemsInvoiceResponse);
    }

}
