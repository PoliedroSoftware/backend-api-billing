using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;

public class WithholdingTaxTotalDto
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }

    [JsonPropertyName("company")]
    public string company { get; set; }

    [JsonPropertyName("invoice")]
    public string invoice { get; set; }

    [JsonPropertyName("is_holding")]
    public bool is_holding { get; set; }

    [JsonPropertyName("tax_id")]
    public int tax_id { get; set; }

    [JsonPropertyName("tax_amount")]
    public string tax_amount { get; set; }

    [JsonPropertyName("percent")]
    public string percent { get; set; }

    [JsonPropertyName("taxable_amount")]
    public string taxable_amount { get; set; }
}