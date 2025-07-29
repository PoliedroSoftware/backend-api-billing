using Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record SenderRequestDTO
{
    public string date { get; init; }
    public string time {  get; init; }
    public string prefix { get; init; }
    public int number {  get; init; }
    public OrderReferenceRequestDTO? orderReference { get; init; }
    public bool send_email { get; init; }
     public AttachmentRequestDTO? attachment1 { get; init; }
     public AttachmentRequestDTO? attachment2 { get; init; }
     public CustomerRequestDTO customer {  get; init; }
     public PaymentRequestDTO payment { get; init; }
     public List<GeneralAllowanceRequestDTO>? generalAllowances { get; init; }
     public List<ItemElectronicRequestDTO> items { get; init; }
     public string resolution { get; init; }
     public string? resolutionText { get; init; }
     public string? head_note { get; init; }
     public string? foot_note { get; init; }
     public string? notes { get; init; }
     public double allowanceTotal { get; init; }
     public double invoiceBaseTotal { get; init; }
     public double invoiceTaxExclusiveTotal { get; init; }
     public double invoiceTaxInclusiveTotal { get; init; }
     public double totalToPay { get; init; }
     public List<AllTaxTotalRequestDTO>? allTaxTotals { get; init; }
     public List<AllHoldingsTaxTotalRequestDTO>? allHoldingsTaxTotals { get; init; }
     public List<CustomSubtotalRequestDTO>? customSubtotals { get; init; }
     public double finalTotalToPay { get; init; }
}