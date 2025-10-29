using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling
{
    // PrepareItemElectronic.cs
    public class PrepareItemElectronic : IPrepareItemBilling
    {
        public async Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items)
        {
            var itemsInvoiceResponse = new List<ItemElectronicEntity>();

            foreach (var item in items)
            {
                // Subtotal (precio × cantidad)
                double subtotal = (double)(item.PriceAmount * item.InvoicedQuantity);

                // Descuento si existe (campo que venga en el item original)
                double discount = item.LineDiscountAmount > 0 ? (double)item.LineDiscountAmount : 0;

                // Base después del descuento (base imponible para calcular impuestos)
                double taxableBase = subtotal - discount;

                // Calcular impuesto sobre la base imponible
                double taxAmount = taxableBase * ((double)item.Percent / 100.0);

                // Redondeos: DIAN trabaja a 2 decimales, hacemos los redondeos por línea
                double taxableBaseRounded = Math.Round(taxableBase, 2, MidpointRounding.AwayFromZero);
                double taxAmountRounded = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero);
                decimal discountRounded = Math.Round((decimal)discount, 2);

                var allowanceCharges = new List<AllowanceChargeEntity>();
                if (discount > 0)
                {
                    allowanceCharges.Add(new AllowanceChargeEntity
                    {
                        ChargeIndicator = false,
                        AllowanceChargeReason = "Discount",
                        MultiplierFactorNumeric = subtotal > 0 ? (decimal)(discount / subtotal) : 0m,
                        Amount = discountRounded,
                        BaseAmount = Math.Round((decimal)subtotal, 2)
                    });
                }

                var taxTotals = new List<TaxTotalEntity>();
                // Si no hay impuesto relevante, puede quedar con percent = 0 y taxAmount = 0
                taxTotals.Add(new TaxTotalEntity
                {
                    TaxId = item.TaxTotals != null && item.TaxTotals.Any() ? item.TaxTotals.First().TaxId : 1,
                    Percent = item.Percent,
                    TaxAmount = taxAmountRounded,
                    TaxableAmount = taxableBaseRounded
                });

                var itemInvoice = new ItemElectronicEntity
                {
                    UnitMeasureId = item.UnitMeasureId > 0 ? item.UnitMeasureId : 70,
                    // CRÍTICO: LineExtensionAmount = base después del descuento (DIAN)
                    LineExtensionAmount = taxableBaseRounded,
                    InvoicedQuantity = item.InvoicedQuantity,
                    FreeOfChargeIndicator = item.FreeOfChargeIndicator,
                    AllowanceCharges = allowanceCharges,
                    TaxTotals = taxTotals,
                    WithHoldingTaxTotal = [],
                    Description = item.Description + (taxAmountRounded > 0 ? $" IVA {taxAmountRounded:F2}" : ""),
                    Notes = item.Notes,
                    Code = item.Code,
                    TypeItemIdentificationId = item.TypeItemIdentificationId,
                    PriceAmount = item.PriceAmount,
                    BaseQuantity = item.BaseQuantity,
                };

                itemsInvoiceResponse.Add(itemInvoice);
            }

            return await Task.FromResult(itemsInvoiceResponse);
        }
    }
}
