using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class TaxTotalDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }

    [JsonPropertyName("company")]
    public string Company { get; set; }

    [JsonPropertyName("invoice")]
    public string Invoice { get; set; }

    [JsonPropertyName("is_holding")]
    public bool IsHolding { get; set; }

    [JsonPropertyName("tax_id")]
    public int TaxId { get; set; }

    [JsonPropertyName("tax_amount")]
    public string TaxAmount { get; set; }

    [JsonPropertyName("percent")]
    public string Percent { get; set; }

    [JsonPropertyName("taxable_amount")]
    public string TaxableAmount { get; set; }
}