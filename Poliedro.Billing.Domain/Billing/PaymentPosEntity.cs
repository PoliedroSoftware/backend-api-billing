using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.BillingPos;

public class PaymentPosEntity
{
    [JsonPropertyName("payment_form_id")]
    public int payment_form_id { get; set; }
    [JsonPropertyName("payment_method_id")]
    public int payment_method_id { get; set; }
    [JsonPropertyName("payment_due_date")]
    public string payment_due_date { get; set; }
    [JsonPropertyName("duration_measure")]
    public string duration_measure { get; set; }
}
