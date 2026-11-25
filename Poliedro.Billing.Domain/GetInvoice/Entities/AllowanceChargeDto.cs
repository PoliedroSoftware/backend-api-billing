using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class AllowanceChargeDto
{
    [JsonPropertyName("charge_indicator")]
    public bool ChargeIndicator { get; set; }

    [JsonPropertyName("discount_id")]
    public int? DiscountId { get; set; }

    [JsonPropertyName("allowance_charge_reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("multiplier_factor_numeric")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Percent { get; set; }

    [JsonPropertyName("base_amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? BaseAmount { get; set; }

    [JsonPropertyName("charge_type")]
    public string ChargeType { get; set; }

    [JsonPropertyName("amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal Amount { get; set; }
}
