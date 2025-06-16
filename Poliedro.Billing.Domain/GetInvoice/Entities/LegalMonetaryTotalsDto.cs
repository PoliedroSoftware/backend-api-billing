using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class LegalMonetaryTotalsDto
{
    [JsonPropertyName("line_extension_amount")]
    public string LineExtensionAmount { get; set; }

    [JsonPropertyName("tax_exclusive_amount")]
    public string TaxExclusiveAmount { get; set; }

    [JsonPropertyName("tax_inclusive_amount")]
    public string TaxInclusiveAmount { get; set; }

    [JsonPropertyName("payable_amount")]
    public string PayableAmount { get; set; }
}
