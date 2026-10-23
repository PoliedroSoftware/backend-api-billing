namespace Poliedro.Billing.Domain.Billing;

public sealed class EmissionFailure
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int ResolutionId { get; set; }
    public string ClientName { get; set; } = "";
    public string InvoiceNumber { get; set; } = "";
    public string Prefix { get; set; } = "";
    public string DocumentType { get; set; } = "";
    public string CommercialMessage { get; set; } = "";
    public string Reason { get; set; } = "";
    public DateTime FailedAt { get; set; }
}
