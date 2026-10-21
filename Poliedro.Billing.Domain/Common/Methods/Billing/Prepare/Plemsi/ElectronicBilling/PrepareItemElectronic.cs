using System.Globalization;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi.TypeTax;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling
{
    public class PrepareItemElectronic : IPrepareItemBilling
    {
        private const int DefaultUnitMeasureId = 70;
        private const int DefaultTaxId = 18;
        private const int ItemIdentificationId = 1;
        private const string DefaultNote = "Description";
        private const string FreeOfChargeSuffix = "(ENTREGA GRATUITA)";

        public async Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items)
        {
            var itemsInvoiceResponse = new List<ItemElectronicEntity>();

            if (items == null || items.Count == 0)
                return itemsInvoiceResponse;

            foreach (var item in items)
            {
                double quantity = item.InvoicedQuantity;
                double grossUnitPrice = item.UnitPriceBeforeDiscount ?? item.UnitPrice;
                double grossAmount = Round(grossUnitPrice * quantity);
                double netAmount = Round(item.UnitPrice * quantity);

                double? sourceDiscountPercentage = LineDiscountReader
                    .ToPercentage(item.LineDiscountType, item.LineDiscountAmount, grossUnitPrice);
                double? sourceDiscountAmount = LineDiscountReader
                    .ToAmount(item.LineDiscountType, item.LineDiscountAmount);

                // unit_price es el precio neto, pero el origen puede informar el descuento sin reflejarlo
                // en ese campo; si el tipo de descuento lo indica y el neto sigue siendo el bruto, se deriva.
                if (sourceDiscountPercentage is double percentage && grossAmount > 0 && netAmount >= grossAmount)
                    netAmount = Round(grossAmount * (1 - Math.Min(percentage, 100) / 100));

                if (netAmount < 0)
                    netAmount = 0;

                bool isFreeItem =
                    netAmount <= 0 ||                                             // valor de línea 0: entrega gratuita
                    (sourceDiscountPercentage ?? 0) >= 100 ||                     // el tipo indica 100% de descuento
                    ((sourceDiscountAmount ?? 0) >= grossAmount && grossAmount > 0); // descuento fijo que cubre la línea

                if (isFreeItem)
                {
                    itemsInvoiceResponse.Add(BuildFreeOfChargeItem(item, grossUnitPrice, quantity));
                    continue;
                }

                WarnIfLineExtensionDiverges(item, netAmount);

                double discountAmount = Round(grossAmount - netAmount);

                var allowanceCharges = new List<AllowanceChargeEntity>();
                if (discountAmount > 0 && grossAmount > 0)
                {
                    // El descuento se declara sobre el bruto: base * factor = amount y base - amount = line_extension.
                    allowanceCharges.Add(new AllowanceChargeEntity
                    {
                        ChargeIndicator = false,
                        AllowanceChargeReason = "Discount",
                        MultiplierFactorNumeric = (decimal)Math.Round(discountAmount / grossAmount, 6, MidpointRounding.AwayFromZero),
                        Amount = (decimal)discountAmount,
                        BaseAmount = (decimal)grossAmount
                    });
                }

                List<TaxTotalEntity> taxTotals = BuildTaxTotals(item, netAmount);

                var itemInvoice = Clone(item);

                itemInvoice.UnitMeasureId = item.UnitMeasureId > 0 ? item.UnitMeasureId : DefaultUnitMeasureId;
                itemInvoice.LineExtensionAmount = netAmount;
                itemInvoice.FreeOfChargeIndicator = false;
                itemInvoice.AllowanceCharges = allowanceCharges;
                itemInvoice.TaxTotals = taxTotals;
                itemInvoice.WithHoldingTaxTotal = new();
                itemInvoice.Description = BuildDescription(item.Description, taxTotals);
                itemInvoice.Notes = string.IsNullOrWhiteSpace(item.Notes) ? DefaultNote : item.Notes;
                itemInvoice.TypeItemIdentificationId = ItemIdentificationId;

                itemsInvoiceResponse.Add(itemInvoice);
            }

            return await Task.FromResult(itemsInvoiceResponse);
        }

        /// <summary>
        /// Un ítem con valor de línea 0 debe declararse como entrega gratuita: sin valor,
        /// sin descuentos y conservando el precio unitario para DIAN.
        /// </summary>
        private static ItemElectronicEntity BuildFreeOfChargeItem(ItemElectronicEntity item, double grossUnitPrice, double quantity)
        {
            var freeItem = Clone(item);

            freeItem.UnitMeasureId = item.UnitMeasureId > 0 ? item.UnitMeasureId : DefaultUnitMeasureId;
            freeItem.LineExtensionAmount = 0;
            freeItem.InvoicedQuantity = quantity;
            freeItem.FreeOfChargeIndicator = true;
            freeItem.AllowanceCharges = new();
            freeItem.TaxTotals = BuildFreeOfChargeTaxTotals(item);
            freeItem.WithHoldingTaxTotal = new();
            freeItem.Description = $"{item.Description} {FreeOfChargeSuffix}";
            freeItem.Notes = string.IsNullOrWhiteSpace(item.Notes) ? DefaultNote : item.Notes;
            freeItem.TypeItemIdentificationId = ItemIdentificationId;
            freeItem.PriceAmount = Round(grossUnitPrice);

            return freeItem;
        }

        private static List<TaxTotalEntity> BuildTaxTotals(ItemElectronicEntity item, double taxableAmount)
        {
            if (item.TaxTotals is not { Count: > 0 })
                return [BuildTaxTotal(DefaultTaxId, item.Percent, taxableAmount, null)];

            return item.TaxTotals
                .Select(sourceTax => BuildTaxTotal(
                    sourceTax.TaxId,
                    sourceTax.Percent > 0 ? sourceTax.Percent : item.Percent,
                    taxableAmount,
                    sourceTax.TaxAmount))
                .ToList();
        }

        /// <summary>
        /// Los ítems gratuitos conservan el impuesto informado por el origen con valores en cero.
        /// </summary>
        private static List<TaxTotalEntity> BuildFreeOfChargeTaxTotals(ItemElectronicEntity item)
        {
            if (item.TaxTotals is not { Count: > 0 })
                return [];

            return item.TaxTotals
                .Select(tax => new TaxTotalEntity
                {
                    TaxId = ResolvePlemsiTaxId(tax.TaxId),
                    Percent = 0,
                    TaxAmount = 0,
                    TaxableAmount = 0
                })
                .ToList();
        }

        private static TaxTotalEntity BuildTaxTotal(int internalTaxId, double percent, double taxableAmount, double? sourceTaxAmount)
        {
            double taxAmount = Round(taxableAmount * (percent / 100));

            if (sourceTaxAmount is double source && source > 0 && Math.Abs(source - taxAmount) > 1)
            {
                Console.WriteLine(
                    $"[PrepareItemElectronic] El impuesto {internalTaxId} calculado ({taxAmount}) " +
                    $"difiere del informado por el origen ({source}).");
            }

            return new TaxTotalEntity
            {
                TaxId = ResolvePlemsiTaxId(internalTaxId),
                Percent = percent,
                TaxAmount = taxAmount,
                TaxableAmount = taxableAmount
            };
        }

        private static int ResolvePlemsiTaxId(int internalTaxId)
        {
            try
            {
                return PlemsiTaxMapping.GetPlemsiId(internalTaxId);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[PrepareItemElectronic] {ex.Message} Se usa el impuesto por defecto ({DefaultTaxId}).");
                return DefaultTaxId;
            }
        }

        private static string BuildDescription(string? description, List<TaxTotalEntity> taxTotals)
        {
            var taxes = taxTotals
                .Where(tax => tax.TaxAmount > 0)
                .Select(tax => $"IVA {tax.TaxAmount.ToString("0.##", CultureInfo.InvariantCulture)}");

            return $"{description} {string.Join(" ", taxes)}";
        }

        private static void WarnIfLineExtensionDiverges(ItemElectronicEntity item, double netAmount)
        {
            if (item.LineExtensionAmount > 0 && netAmount > 0 && Math.Abs(item.LineExtensionAmount - netAmount) > 1)
            {
                Console.WriteLine(
                    $"[PrepareItemElectronic] Ítem {item.Code}: el valor de línea informado ({item.LineExtensionAmount}) " +
                    $"difiere del calculado ({netAmount}).");
            }
        }

        /// <summary>
        /// Conserva todos los datos del ítem de origen (transacción, descuentos, precios, etc.)
        /// para que solo se sobrescriban los campos que se calculan en la preparación.
        /// </summary>
        private static ItemElectronicEntity Clone(ItemElectronicEntity item) => new()
        {
            UnitMeasureId = item.UnitMeasureId,
            LineExtensionAmount = item.LineExtensionAmount,
            LineDiscountAmount = item.LineDiscountAmount,
            LineDiscountType = item.LineDiscountType,
            UnitPriceBeforeDiscount = item.UnitPriceBeforeDiscount,
            Transaccion = item.Transaccion,
            FreeOfChargeIndicator = item.FreeOfChargeIndicator,
            AllowanceCharges = item.AllowanceCharges,
            TaxTotals = item.TaxTotals,
            WithHoldingTaxTotal = item.WithHoldingTaxTotal,
            Description = item.Description,
            Notes = item.Notes,
            Code = item.Code,
            TypeItemIdentificationId = item.TypeItemIdentificationId,
            PriceAmount = item.PriceAmount,
            BaseQuantity = item.BaseQuantity,
            InvoicedQuantity = item.InvoicedQuantity,
            Percent = item.Percent,
            TaxAmount = item.TaxAmount,
            UnitPrice = item.UnitPrice,
            Subtotal = item.Subtotal
        };

        private static double Round(double value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
