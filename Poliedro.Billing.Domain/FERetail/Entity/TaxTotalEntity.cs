using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class TaxTotalEntity
{

    [JsonPropertyName("tax_id")]
    public int tax_id { get; set; }

    [JsonPropertyName("percent")]
    public double percent { get; set; }

    [JsonPropertyName("tax_amount")]
    public double tax_amount { get; set; }

    [JsonPropertyName("taxable_amount")]
    public double taxable_amount { get; set; }
}
