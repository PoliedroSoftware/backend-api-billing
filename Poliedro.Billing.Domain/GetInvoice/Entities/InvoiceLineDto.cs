using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;

public class InvoiceLineDto
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }

    [JsonPropertyName("company")]
    public string Company { get; set; }

    [JsonPropertyName("invoice")]
    public string Invoice { get; set; }

    [JsonPropertyName("unit_measure_id")]
    public int UnitMeasureId { get; set; }

    [JsonPropertyName("invoiced_quantity")]
    public string InvoicedQuantity { get; set; }

    [JsonPropertyName("line_extension_amount")]
    public string LineExtensionAmount { get; set; }

    [JsonPropertyName("free_of_charge_indicator")]
    public bool FreeOfChargeIndicator { get; set; }

    [JsonPropertyName("tax_totals")]
    public List<TaxTotalDto> TaxTotals { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("type_item_identification_id")]
    public int TypeItemIdentificationId { get; set; }

    [JsonPropertyName("price_amount")]
    public string PriceAmount { get; set; }

    [JsonPropertyName("base_quantity")]
    public string BaseQuantity { get; set; }

    [JsonPropertyName("with_holding_tax_total")]
    public List<WithholdingTaxTotalDto> WithHoldingTaxTotal { get; set; }
}
