namespace Poliedro.Billing.Application.LastInvoiceNumber.Dtos;

public class LastInvoiceNumberDto
{
    public int NextInvoiceNumber { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public string ResolutionNumber { get; set; } = string.Empty;
    public int CurrentlyNumber { get; set; }
    public int FinalRange { get; set; }
    public DateTime ExpirationDate { get; set; }
}
