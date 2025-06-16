namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class PaymentForm
{
    public int payment_form_id { get; set; }
    public int payment_method_id { get; set; }
    public string payment_due_date { get; set; } = default!;
}
