using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class ItemElectronicEntity
{

    [JsonPropertyName("unit_measure_id")]
    public int unit_measure_id { get; set; }

    [JsonPropertyName("line_extension_amount")]
    public double line_extension_amount { get; set; }

    [JsonPropertyName("free_of_charge_indicator")]
    public bool free_of_charge_indicator { get; set; }

    [JsonPropertyName("allowance_charges")]
    public List<AllowanceChargeEntity>? allowance_charges { get; set; }

    [JsonPropertyName("tax_totals")]
    public List<TaxTotalEntity>? tax_totals { get; set; }

    [JsonPropertyName("with_holding_tax_total")]
    public List<WIthHoldingTaxTotalEntity>? with_holding_tax_total { get; set; }

    [JsonPropertyName("description")]
    public string? description { get; set; }

    [JsonPropertyName("notes")]
    public string? notes { get; set; }

    [JsonPropertyName("code")]
    public string? code { get; set; }

    [JsonPropertyName("type_item_identification_id")]
    public int type_item_identification_id { get; set; }

    [JsonPropertyName("price_amount")]
    public double price_amount { get; set; }

    [JsonPropertyName("base_quantity")]
    public double base_quantity { get; set; }

    [JsonPropertyName("invoiced_quantity")]
    public double invoiced_quantity { get; set; }
}
