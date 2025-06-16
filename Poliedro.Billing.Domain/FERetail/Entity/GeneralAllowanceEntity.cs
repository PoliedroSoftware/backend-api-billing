using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class GeneralAllowanceEntity
{
    [JsonPropertyName("allowance_charge_reason")]
    public string? allowance_charge_reason { get; set; }

    [JsonPropertyName("allowance_percent")]
    public double? allowance_percent { get; set; }

    [JsonPropertyName("amount")]
    public required decimal amount { get; set; }

    [JsonPropertyName("base_amount")]
    public required decimal base_amount { get; set; }
}
