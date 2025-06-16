using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.BillingPos;

public class ItemFERetailEntity
{
    [JsonPropertyName("unit_measure_id")]
    public int unit_measure_id { get; set; }
    [JsonPropertyName("invoiced_quantity")]
    public string invoiced_quantity { get; set; }
    [JsonPropertyName("line_extension_amount")]
    public string line_extension_amount { get; set; }
    [JsonPropertyName("free_of_charge_indicator")]
    public bool free_of_charge_indicator { get; set; }
    [JsonPropertyName("tax_totals")]
    public List<TaxTotalPosEntity> tax_totals { get; set; }
    [JsonPropertyName("description")]
    public string description { get; set; }
    [JsonPropertyName("notes")]
    public string notes { get; set; }
    [JsonPropertyName("code")]
    public string code { get; set; }
    [JsonPropertyName("type_item_identification_id")]
    public int type_item_identification_id { get; set; }
    [JsonPropertyName("price_amount")]
    public string price_amount { get; set; }
    [JsonPropertyName("base_quantity")]
    public string base_quantity { get; set; }
}
