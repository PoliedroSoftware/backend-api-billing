using Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record SenderRequestFEDTO
{
    public string date { get; init; }
    public string time {  get; init; }
    public string prefix { get; init; }
    public int number {  get; set; }
    public OrderReferenceRequestFEDTO? orderReference { get; init; }
    public bool send_email { get; init; }
     public AttachmentRequestFEDTO? attachment1 { get; init; }
     public AttachmentRequestFEDTO? attachment2 { get; init; }
     public required CustomerRequestFEDTO customer {  get; init; }
     public required PaymentRequestFEDTO payment { get; init; }
     public List<GeneralAllowanceRequestFEDTO>? generalAllowances { get; init; }
     public required List<ItemElectronicRequestFEDTO> items { get; init; }
     public required string resolution { get; init; }
     public string? resolutionText { get; init; }
     public string? head_note { get; init; }
     public string? foot_note { get; init; }
     public string? notes { get; init; }
     public double allowanceTotal { get; init; }
     public double invoiceBaseTotal { get; init; }
     public double invoiceTaxExclusiveTotal { get; init; }
     public double invoiceTaxInclusiveTotal { get; init; }
     public double totalToPay { get; init; }
     public List<AllTaxTotalRequestFEDTO>? allTaxTotals { get; init; }
     public List<AllHoldingsTaxTotalRequestFEDTO>? allHoldingsTaxTotals { get; init; }
     public List<CustomSubtotalRequestFEDTO>? customSubtotals { get; init; }
     public double finalTotalToPay { get; init; }
}