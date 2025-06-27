using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class WIthHoldingTaxTotalEntity
{
    [JsonPropertyName("tax_id")]
    public int TaxId { get; set; }

    [JsonPropertyName("percent")]
    public int Percent { get; set; }

    [JsonPropertyName("tax_amount")]
    public int TaxAmount { get; set; }

    [JsonPropertyName("taxable_amount")]
    public int TaxableAmount { get; set; }

}
