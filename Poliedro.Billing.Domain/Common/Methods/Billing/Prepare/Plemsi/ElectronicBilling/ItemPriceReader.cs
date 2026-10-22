using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;

/// <summary>
/// Resuelve el precio unitario de referencia que se informa a la DIAN.
/// Para las muestras o regalos sin valor comercial la DIAN exige un precio de
/// referencia mayor que cero (regla FAW03) aunque el valor de la línea sea cero.
/// </summary>
internal static class ItemPriceReader
{
    /// <summary>
    /// Busca el primer precio unitario positivo disponible en el detalle de la factura.
    /// Devuelve false cuando el origen no informó ningún precio.
    /// </summary>
    public static bool TryResolveReferenceUnitPrice(ItemElectronicEntity item, out double unitPrice)
    {
        if (item.UnitPriceBeforeDiscount is > 0 and double beforeDiscount)
        {
            unitPrice = Round(beforeDiscount);
            return true;
        }

        if (item.PriceAmount > 0)
        {
            unitPrice = Round(item.PriceAmount);
            return true;
        }

        if (item.UnitPrice > 0)
        {
            unitPrice = Round(item.UnitPrice);
            return true;
        }

        // La línea trae valor pero no precio unitario: se reconstruye el unitario
        // solo si no hay descuento, para no tomar el valor neto como precio de lista.
        if (item.LineExtensionAmount > 0 &&
            item.InvoicedQuantity > 0 &&
            (item.LineDiscountAmount ?? 0) <= 0)
        {
            unitPrice = Round(item.LineExtensionAmount / item.InvoicedQuantity);
            return true;
        }

        unitPrice = 0;
        return false;
    }

    private static double Round(double value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
