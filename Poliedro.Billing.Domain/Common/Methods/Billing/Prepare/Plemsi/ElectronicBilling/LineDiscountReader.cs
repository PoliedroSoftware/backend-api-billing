using System.Globalization;
using System.Text;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;

/// <summary>
/// Normaliza e interpreta el tipo de descuento que llega desde el detalle de la factura
/// (v_invoice_detail.line_discount_type). El origen no garantiza el formato: puede venir en
/// mayúsculas, con espacios, con acentos o sin ellos, y el descuento puede estar expresado
/// como porcentaje ("percentage") o como monto ("fixed").
/// </summary>
internal static class LineDiscountReader
{
    private static readonly string[] PercentageAliases =
        ["percentage", "percent", "porcentaje", "porcentual", "%"];

    private static readonly string[] FixedAliases =
        ["fixed", "fijo", "fija", "valor", "value", "amount", "monto"];

    public static bool IsPercentage(string? discountType)
        => PercentageAliases.Contains(Normalize(discountType));

    public static bool IsFixed(string? discountType)
        => FixedAliases.Contains(Normalize(discountType));

    /// <summary>
    /// Devuelve el porcentaje de descuento (0 - 100) cuando el tipo informado es porcentual.
    /// Si el origen reporta el descuento como monto (por ejemplo 15000 sobre un bruto de 15000)
    /// se convierte a porcentaje. Devuelve null cuando el tipo no es reconocido o no hay descuento.
    /// </summary>
    public static double? ToPercentage(string? discountType, double? discountAmount, double grossUnitPrice)
    {
        if (discountAmount is not double amount || amount <= 0 || !IsPercentage(discountType))
            return null;

        return amount > 100 && grossUnitPrice > 0
            ? Math.Round(amount / grossUnitPrice * 100, 5, MidpointRounding.AwayFromZero)
            : amount;
    }

    /// <summary>
    /// Devuelve el descuento expresado como monto cuando el tipo informado es fijo ("fixed").
    /// </summary>
    public static double? ToAmount(string? discountType, double? discountAmount)
        => IsFixed(discountType) && discountAmount is double amount && amount > 0 ? amount : null;

    /// <summary>
    /// Deja el tipo de descuento en minúsculas, sin espacios extremos y sin diacríticos,
    /// para que la comparación no dependa del formato con que lo entregue el origen.
    /// </summary>
    private static string Normalize(string? discountType)
    {
        if (string.IsNullOrWhiteSpace(discountType))
            return string.Empty;

        string decomposed = discountType.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (char character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        return builder.ToString().Trim();
    }
}
