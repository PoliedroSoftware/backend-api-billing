using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class GeneralAllowanceEntity
{
    [JsonPropertyName("allowance_charge_reason")]
    public string? AllowanceChargeReason { get; set; }

    [JsonPropertyName("allowance_percent")]
    public double? AllowancePercent { get; set; }

    [JsonPropertyName("multiplier_factor_numeric")]
    public decimal MultiplierFactorNumeric { get; set; }

    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }

    [JsonPropertyName("base_amount")]
    public required decimal BaseAmount { get; set; }
}
