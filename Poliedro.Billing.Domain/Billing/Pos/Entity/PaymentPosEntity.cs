

namespace Poliedro.Billing.Domain.Billing.Pos.Entity;
public class PaymentPosEntity
{
    public int payment_form_id { get; set; }
    public int payment_method_id { get; set; }
    public string payment_due_date { get; set; }
    public string duration_measure { get; set; }
}
