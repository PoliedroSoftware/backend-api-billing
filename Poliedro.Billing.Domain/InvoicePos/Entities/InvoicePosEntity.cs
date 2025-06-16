namespace Poliedro.Billing.Domain.InvoicePos.Entities;
public class InvoicePosEntity
{
    public PaymentForm payment_form { get; set; } = default!;
    public LegalMonetaryTotals legal_monetary_totals { get; set; } = default!;
    public string _id { get; set; } = default!;
    public string company { get; set; } = default!;
    public string state { get; set; } = default!;
    public Response response { get; set; } = default!;
    public string generatedBy { get; set; } = default!;
    public string resolution { get; set; } = default!;
    public int type_document_id { get; set; } = default!;
    public string date { get; set; } = default!;
    public string time { get; set; } = default!;
    public string notes { get; set; } = default!;
    public string head_note { get; set; } = default!;
    public string foot_note { get; set; } = default!;
    public string resolution_number { get; set; } = default!;
    public string prefix { get; set; } = default!;
    public string number { get; set; } = default!;
    public List<TaxTotal> tax_totals { get; set; } = default!;
    public List<object> with_holding_tax_total { get; set; } = default!;
    public List<InvoiceLine> invoice_lines { get; set; } = default!;
    public AdditionalData additionalData { get; set; } = default!;
    public string consecutive { get; set; } = default!;
    public List<object> allowance_charges { get; set; } = default!;
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public string QRStr { get; set; } = default!;
    public string cude { get; set; } = default!;
} 