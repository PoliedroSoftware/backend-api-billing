using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.BillingPos;

public class TaxTotalPosEntity
{
    [JsonPropertyName("tax_id")]
    public int tax_id { get; set; }
    [JsonPropertyName("tax_amount")]
    public int tax_amount { get; set; }
    [JsonPropertyName("percent")]
    public int percent { get; set; }
    [JsonPropertyName("taxable_amount")]
    public decimal taxable_amount { get; set; }
}
