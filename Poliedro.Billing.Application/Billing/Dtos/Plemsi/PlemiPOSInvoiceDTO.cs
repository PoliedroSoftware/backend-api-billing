namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;

public record PlemiPOSInvoiceDTO
{
    public int Number { get; init; }
    public DateTime Date { get; init; }
    public DateTime Time { get; init; }
    public string? SendToEmail { get; init; }
    public SoftwareManufacturerDTO? SoftwareManufacturer { get; init; }
    public CustomerBillingDTO? CustomerEntity { get; init; }
    public PayPointInfoDTO? PayPointInfoEntity { get; init; }
    public int Resolution { get; init; }
    public string Prefix { get; init; }
    public string? HeadNote { get; init; }
    public string? FootNote { get; init; }
    public PaymentDTO? PaymentEntity { get; init; }
    public decimal InvoiceBaseTotal { get; init; }
    public decimal InvoiceTaxExclusiveTotal { get; init; }
    public decimal InvoiceTaxInclusiveTotal { get; init; }
    public decimal TotalToPay { get; init; }
    public List<AllTaxTotalDTO>? AllTaxTotalEntity { get; init; }
    public List<ItemElectronicDTO>? ItemElectronicEntity { get; init; }
}
