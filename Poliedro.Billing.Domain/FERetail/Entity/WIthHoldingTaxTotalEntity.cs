using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class WIthHoldingTaxTotalEntity
{
    [JsonPropertyName("tax_id")]
    public int tax_id { get; set; }

    [JsonPropertyName("percent")]
    public int percent { get; set; }

    [JsonPropertyName("tax_amount")]
    public int tax_amount { get; set; }

    [JsonPropertyName("taxable_amount")]
    public int taxable_amount { get; set; }

}
