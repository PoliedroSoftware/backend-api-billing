using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class AllowanceCharge
{
    [JsonPropertyName("charge_type")]
    public string ChargeType { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
