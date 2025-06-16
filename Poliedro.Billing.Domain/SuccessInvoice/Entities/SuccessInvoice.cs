namespace Poliedro.Billing.Domain.SuccessInvoice.Entities;

public class SuccessInvoice
{
    public required string _Id { get; set; }
    public required string Company { get; set; }
    public required string CreatedBy { get; set; }
    public required string State { get; set; }
    public string? Preview { get; set; }
    public required string Resolution_Number { get; set; }
    public string? Prefix { get; set; }
    public required string Number { get; set; }
    public int Type_Document_Id { get; set; }
    public required string Date { get; set; }
    public required string Time { get; set; }
    public required Customer Customer { get; set; }
    public OrderReference? Order_Reference { get; set; }
    public PaymentForm? Payment_Form { get; set; }
    public required LegalMonetaryTotals Legal_Monetary_Totals { get; set; }
    public string? EmailDeliveryStatus { get; set; }
    public bool SendEmailNotification { get; set; }
    public string? Consecutive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Cude { get; set; }
    public Response? Response { get; set; }
}