using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity
{
    public class PaymentEntity
    {
        [JsonPropertyName("payment_form_id")]
        public int PaymentFormId { get; set; }
        [JsonPropertyName("payment_method_id")]
        public int PaymentMethodId { get; set; }
        [JsonPropertyName("payment_due_date")]
        public string? PaymentDueDate { get; set; }
        [JsonPropertyName("duration_measure")]
        public string? DurationMeasure { get; set; }
    }
}
