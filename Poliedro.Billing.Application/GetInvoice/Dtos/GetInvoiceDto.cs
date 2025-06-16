namespace Poliedro.Billing.Domain.GetInvoice.Entities;

public class GetInvoiceDto
{
    public OrderReferenceDto OrderReference { get; set; }
    public LegalMonetaryTotalsDto LegalMonetaryTotals { get; set; }
    public string Id { get; set; }
    public string Company { get; set; }
    public string CreatedBy { get; set; }
    public string State { get; set; }
    public string Preview { get; set; }
    public string ResolutionNumber { get; set; }
    public string Prefix { get; set; }
    public string Number { get; set; }
    public int TypeDocumentId { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string EmailDeliveryStatus { get; set; }
    public bool SendEmailNotification { get; set; }
    public string Consecutive { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public ResponseDto Response { get; set; }
    public CustomerDto Customer { get; set; }
    public List<PaymentFormDto> PaymentForm { get; set; }
    public List<TaxTotalDto> TaxTotals { get; set; }
    public List<WithholdingTaxTotalDto> WithHoldingTaxTotal { get; set; }
    public string HeadNote { get; set; }
    public string FootNote { get; set; }
    public string Notes { get; set; }
    public string Resolution { get; set; }
    public string QRStr { get; set; }
    public List<AllowanceChargeDto> AllowanceCharges { get; set; }
    public List<InvoiceLineDto> InvoiceLine { get; set; }
    public List<AttachmentDto> Attachment { get; set; }
    public string TechProviderDefaultFootNote { get; set; }
}
