using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity
{
    public class AllowanceChargeEntity
    {
        [JsonPropertyName("charge_indicator")]
        public bool charge_indicator { get; set; }

        [JsonPropertyName("allowance_charge_reason")]
        public string? allowance_charge_reason { get; set; }

        [JsonPropertyName("multiplier_factor_numeric")]
        public decimal multiplier_factor_numeric { get; set; }

        [JsonPropertyName("amount")]
        public decimal amount { get; set; }

        [JsonPropertyName("base_amount")]
        public decimal base_amount { get; set; }
        
    }
}
