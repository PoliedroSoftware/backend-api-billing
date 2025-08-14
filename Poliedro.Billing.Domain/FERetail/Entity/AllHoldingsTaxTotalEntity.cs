using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity
{
    public class AllHoldingsTaxTotalEntity
    {
        [JsonPropertyName("tax_id")]
        public int TaxId { get; set; }

        [JsonPropertyName("tax_amount")]
        public double TaxAmount { get; set; }

        [JsonPropertyName("percent")]
        public double Percent { get; set; }

        [JsonPropertyName("taxable_amount")]
        public double TaxableAmount { get; set; }

    }
}
