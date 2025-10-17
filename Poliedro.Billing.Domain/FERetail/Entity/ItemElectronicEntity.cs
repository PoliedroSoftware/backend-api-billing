using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class ItemElectronicEntity
{

    [JsonPropertyName("unit_measure_id")]
    public int UnitMeasureId { get; set; }

    [JsonPropertyName("line_extension_amount")]
    public double LineExtensionAmount { get; set; }
    public double LineDiscountAmount { get; set; }

    [JsonPropertyName("transaccion")]
    public int? Transaccion { get; set; }

    [JsonPropertyName("free_of_charge_indicator")]
    public bool FreeOfChargeIndicator { get; set; }

    [JsonPropertyName("allowance_charges")]
    public List<AllowanceChargeEntity>? AllowanceCharges { get; set; }

    [JsonPropertyName("tax_totals")]
    public List<TaxTotalEntity>? TaxTotals { get; set; }

    [JsonPropertyName("with_holding_tax_total")]
    public List<WIthHoldingTaxTotalEntity>? WithHoldingTaxTotal { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("type_item_identification_id")]
    public int TypeItemIdentificationId { get; set; }

    [JsonPropertyName("price_amount")]
    public double PriceAmount { get; set; }

    [JsonPropertyName("base_quantity")]
    public double BaseQuantity { get; set; }

    [JsonPropertyName("invoiced_quantity")]
    public double InvoicedQuantity { get; set; }

    [JsonPropertyName("percent")]
    public double Percent { get; set; }

    [JsonPropertyName("tax_amount")]
    public double TaxAmount { get; set; }

    [JsonPropertyName("unit_price")]
    public double UnitPrice { get; set; }

    [JsonPropertyName("subtotal")]
    public double Subtotal { get; set; }
}
