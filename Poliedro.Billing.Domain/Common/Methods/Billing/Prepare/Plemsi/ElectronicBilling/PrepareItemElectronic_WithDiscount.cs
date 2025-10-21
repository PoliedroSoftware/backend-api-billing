using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
using System;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;

public class PrepareItemElectronic_WithDiscount : IPrepareItemBilling
{
    public async Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items)
    {
        List<ItemElectronicEntity> itemsInvoiceResponse = [];

        foreach (ItemElectronicEntity item in items)
        {
            // Calcular el precio unitario final después de aplicar el descuento
            double finalUnitPrice = CalculateFinalPrice(
                item.UnitPriceBeforeDiscount,
                item.LineDiscountAmount,
                item.LineDiscountType
            );

            var itemInvoice = new ItemElectronicEntity
            {
                UnitMeasureId = 70,
                
                // Usar el precio final calculado para LineExtensionAmount
                LineExtensionAmount = finalUnitPrice * item.InvoicedQuantity,
                
                InvoicedQuantity = item.InvoicedQuantity,
                FreeOfChargeIndicator = false,

                AllowanceCharges = [],

                TaxTotals = [
                     new TaxTotalEntity {
                        TaxId = 1,
                        Percent = item.Percent,
                        TaxAmount = item.TaxAmount * item.InvoicedQuantity,
                        
                        // Usar el precio final para calcular el TaxableAmount
                        TaxableAmount = finalUnitPrice * item.InvoicedQuantity
                }],

                WithHoldingTaxTotal = [],

                Description = $"{item.Description} {(item.TaxTotals?.FirstOrDefault()?.TaxAmount > 0 ? $"IVA {item.TaxTotals?.FirstOrDefault()?.TaxAmount}" : "")}",
                Notes = "",
                Code = item.Code,
                TypeItemIdentificationId = 1,
                
                // PriceAmount es el precio unitario final después del descuento
                PriceAmount = finalUnitPrice,
                
                BaseQuantity = item.InvoicedQuantity,
                
                // Mantener el precio original antes del descuento
                UnitPriceBeforeDiscount = item.UnitPriceBeforeDiscount,
                
                // Mantener la información del descuento
                LineDiscountAmount = item.LineDiscountAmount,
                LineDiscountType = item.LineDiscountType
            };
            
            itemsInvoiceResponse.Add(itemInvoice);
        }

        return await Task.FromResult(itemsInvoiceResponse);
    }

    /// <summary>
    /// Calcula el precio final después de aplicar el descuento
    /// </summary>
    /// <param name="unitPriceBeforeDiscount">Precio unitario antes del descuento</param>
    /// <param name="lineDiscountAmount">Monto o porcentaje del descuento</param>
    /// <param name="lineDiscountType">Tipo de descuento: "Percentage", "FixedAmount", "Porcentaje", "Valor", etc.</param>
    /// <returns>Precio unitario final después de aplicar el descuento</returns>
    private double CalculateFinalPrice(double unitPriceBeforeDiscount, double lineDiscountAmount, string? lineDiscountType)
    {
        // Si no hay descuento, retornar el precio original
        if (lineDiscountAmount <= 0 || string.IsNullOrWhiteSpace(lineDiscountType))
        {
            return unitPriceBeforeDiscount;
        }

        // Normalizar el tipo de descuento a minúsculas para comparación
        string discountType = lineDiscountType.Trim().ToLower();

        // Calcular según el tipo de descuento
        switch (discountType)
        {
            case "percentage":
            case "porcentaje":
            case "%":
            case "percent":
                // Descuento por porcentaje: restar el porcentaje del precio
                double discountAmount = unitPriceBeforeDiscount * (lineDiscountAmount / 100.0);
                return unitPriceBeforeDiscount - discountAmount;

            case "fixedamount":
            case "valor":
            case "fixed":
            case "amount":
            case "fijo":
                // Descuento por valor fijo: restar directamente el monto
                return unitPriceBeforeDiscount - lineDiscountAmount;

            default:
                // Si el tipo no es reconocido, asumir valor fijo
                return unitPriceBeforeDiscount - lineDiscountAmount;
        }
    }
}
