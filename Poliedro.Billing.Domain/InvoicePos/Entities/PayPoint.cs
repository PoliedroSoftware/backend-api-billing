namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class PayPoint
{
    public string code { get; set; } = default!;
    public string address { get; set; } = default!;
    public string cashier { get; set; } = default!;
    public string type { get; set; } = default!;
    public string sale_code { get; set; } = default!;
}
