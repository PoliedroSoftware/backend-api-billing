namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;

public record PlemiPOSInvoiceDTO
{
    public required string Number { get; init; }

    public required DateTime TransactionDate { get; init; }
    public DateTime Date { get; init; }
    public DateTime Time { get; init; }
    public string? SendToEmail { get; init; }
    public SoftwareManufacturerDTO? SoftwareManufacturer { get; init; }
    public CustomerBillingDTO? CustomerEntity { get; init; }
    public PayPointInfoDTO? PayPointInfoEntity { get; init; }
    public required string Resolution { get; init; }
    public required string Prefix { get; init; }
    public string? HeadNote { get; init; }
    public string? FootNote { get; init; }
    public PaymentDTO? PaymentEntity { get; init; }
    public double InvoiceBaseTotal { get; init; }
    public double InvoiceTaxExclusiveTotal { get; init; }
    public double InvoiceTaxInclusiveTotal { get; init; }
    public double TotalToPay { get; init; }
    public List<AllTaxTotalDTO>? AllTaxTotalEntity { get; init; }
    public List<ItemElectronicDTO>? ItemElectronicEntity { get; init; }
}
