using Poliedro.Billing.Domain.InvoicePos.Entities;

namespace Poliedro.Billing.Domain.SuccessInvoice.Entities;

public class ResponseData
{
    public string? IsValid { get; set; }
    public string? StatusDescription { get; set; }
    public ErrorMessage? ErrorMessage { get; set; }
    public string? IssueDate { get; set; }
    public string? IssueTime { get; set; }
    public string? Filename { get; set; }
}
