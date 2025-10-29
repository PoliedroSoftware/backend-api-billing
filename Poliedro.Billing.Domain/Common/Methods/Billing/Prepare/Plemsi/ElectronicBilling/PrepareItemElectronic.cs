using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
using System;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;

public class PrepareItemElectronic(
) : IPrepareItemBilling
{
    public async Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items)
    {
        List<ItemElectronicEntity> itemsInvoiceResponse = [];

        foreach (ItemElectronicEntity item in items)
        {
            // Subtotal ANTES de descuento (este es el LineExtensionAmount según DIAN)
            double subtotal = item.UnitPrice * item.InvoicedQuantity;

            // Descuento aplicado
            double discount = (double)(item.LineDiscountAmount > 0 ? item.LineDiscountAmount : 0);

            // Base DESPUÉS del descuento (para calcular impuestos)
            double baseAfterDiscount = subtotal - discount;

            // Impuesto sobre la base después del descuento
            double taxAmount = baseAfterDiscount * (item.Percent / 100);

            // Calcular el porcentaje de descuento
            double discountPercent = subtotal > 0 ? (discount / subtotal) * 100 : 0;
            double roundedDiscountPercent = Math.Round(discountPercent, 2);

            var itemInvoice = new ItemElectronicEntity
            {
                UnitMeasureId = 70,
                // CRÍTICO: LineExtensionAmount debe ser el valor ANTES de descuentos según FAU02
                LineExtensionAmount = subtotal,  // NO restar el descuento aquí
                InvoicedQuantity = item.InvoicedQuantity,
                FreeOfChargeIndicator = false,

                // Los descuentos van SOLO en allowance_charges
                AllowanceCharges = discount > 0
                    ? [
                        new AllowanceChargeEntity {
                        ChargeIndicator = false,
                        AllowanceChargeReason = "Discount",
                        MultiplierFactorNumeric = (decimal)(discountPercent / 100), // Como decimal 0-1
                        Amount = (decimal)discount,
                        BaseAmount = (decimal)subtotal
                    }
                    ]
                    : [],

                // Los impuestos se calculan sobre la base DESPUÉS de descuentos
                TaxTotals = [
                    new TaxTotalEntity {
                    TaxId = 1,
                    Percent = item.Percent,
                    TaxAmount = taxAmount,
                    TaxableAmount = baseAfterDiscount  // Base después de descuento
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