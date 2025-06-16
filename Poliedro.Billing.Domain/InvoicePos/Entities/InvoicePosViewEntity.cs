namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class InvoicePosViewEntity
{
    public string Id { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string GeneratedBy { get; set; } = string.Empty;
    public string Resolution { get; set; } = string.Empty;
    public string ResolutionNumber { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public int TypeDocumentId { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string HeadNote { get; set; } = string.Empty;
    public string FootNote { get; set; } = string.Empty;
    public string Consecutive { get; set; } = string.Empty;
    public string QRStr { get; set; } = string.Empty;
    public string Cude { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public decimal LineExtensionAmount { get; set; }
    public decimal TaxExclusiveAmount { get; set; }
    public decimal TaxInclusiveAmount { get; set; }
    public decimal PayableAmount { get; set; }

    public int PaymentFormId { get; set; }
    public int PaymentMethodId { get; set; }
    public string PaymentDueDate { get; set; } = string.Empty;

    public ICollection<object> TaxTotals { get; set; } = new List<object>();
    public ICollection<object> WithHoldingTaxTotal { get; set; } = new List<object>();
    public ICollection<object> AllowanceCharges { get; set; } = new List<object>();
    public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
