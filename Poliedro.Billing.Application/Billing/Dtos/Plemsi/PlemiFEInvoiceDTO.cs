namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;

public record PlemiFEInvoiceDTO
{
    public DateTime Date { get; init; }
    public DateTime Time { get; init; }
    public required string Number { get; init; }
    public OrderReferenceDTO? OrderReference { get; init; }
    public bool SendEmail { get; init; }
    public AttachmentDTO? Attachment1 { get; init; }
    public AttachmentDTO? Attachment2 { get; init; }
    public CustomerBillingDTO? CustomerEntity { get; init; }
    public PaymentDTO? PaymentEntity { get; init; }
    public List<GeneralAllowanceDTO>? GeneralAllowanceEntity { get; init; }
    public List<ItemElectronicDTO>? ItemElectronicEntity { get; init; }
    public decimal Resolution { get; init; }
    public string? HeadNote { get; init; }
    public string? FootNote { get; init; }
    public string? Notes { get; init; }
    public decimal AllowanceTotal { get; init; }
    public decimal InvoiceBaseTotal { get; init; }
    public decimal InvoiceTaxExclusiveTotal { get; init; }
    public decimal InvoiceTaxInclusiveTotal { get; init; }
    public decimal TotalToPay { get; init; }
    public List<AllTaxTotalDTO>? AllTaxTotalEntity { get; init; }
    public List<AllHoldingsTaxTotalDTO>? AllHoldingsTaxTotalEntity { get; init; }
    public decimal FinalTotalToPay { get; init; }
}
