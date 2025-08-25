namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.POS;
public record InvoiceRequestPosDto
{
    public int number { get; set; }
    public required string date { get; init; }
    public required string time { get; init; }
    public SoftwareManufacturerRequestPosDto softwareManufacturer { get; init; }
    public required string sendToEmail { get; init; }
    public required string resolution { get; init; }
    public required string prefix { get; init; }
    public string head_note { get; init; }
    public string foot_note { get; init; }
    public PayPointInfoRequestPosDto payPointInfo { get; init; }
    public PaymentRequestPosDto payment { get; init; }
    public required string invoiceBaseTotal { get; init; }
    public required string invoiceTaxExclusiveTotal { get; init; }
    public string invoiceTaxInclusiveTotal { get; init; }
    public string totalToPay { get; init; }
    public List<TaxTotalRequestPosDto> allTaxTotals { get; init; }
    public List<ItemRequestPosDto> items { get; init; }
}
