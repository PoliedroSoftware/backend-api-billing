namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class ResponseData
{
    public string IsValid { get; set; } = default!;
    public string StatusDescription { get; set; } = default!;
    public ErrorMessage ErrorMessage { get; set; } = default!;
    public string IssueDate { get; set; } = default!;
    public string IssueTime { get; set; } = default!;
    public string Filename { get; set; } = default!;
}
