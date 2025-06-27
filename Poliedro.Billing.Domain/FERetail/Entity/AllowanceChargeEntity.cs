using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity
{
    public class AllowanceChargeEntity
    {
        [JsonPropertyName("charge_indicator")]
        public bool ChargeIndicator { get; set; }

        [JsonPropertyName("allowance_charge_reason")]
        public string? AllowanceChargeReason { get; set; }

        [JsonPropertyName("multiplier_factor_numeric")]
        public decimal MultiplierFactorNumeric { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("base_amount")]
        public decimal BaseAmount { get; set; }
        
    }
}
