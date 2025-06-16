using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class PaymentFormDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }

    [JsonPropertyName("company")]
    public string Company { get; set; }

    [JsonPropertyName("invoice")]
    public string Invoice { get; set; }

    [JsonPropertyName("payment_form_id")]
    public int PaymentFormId { get; set; }

    [JsonPropertyName("payment_method_id")]
    public int PaymentMethodId { get; set; }

    [JsonPropertyName("payment_due_date")]
    public string PaymentDueDate { get; set; }

    [JsonPropertyName("paid_amount")]
    public string PaidAmount { get; set; }
}
