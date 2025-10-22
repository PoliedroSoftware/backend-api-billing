using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Billing;

public class CreateBilling
{
    public DateTime Date { get; set; }
    public DateTime Time { get; set; }
    public string? SendToEmail { get; set; }
    public SoftwareManufacturerEntity? SoftwareManufacturer { get; set; }
    public PayPointInfoEntity? PayPointInfo { get; set; }
    public required string Number { get; set; }
    public string? Numeration { get; set; } 
    public string? Prefix { get; set; }
    public DateTime? TransactionDate { get; set; }
    public OrderReferenceEntity? OrderReference { get; set; }
    public bool? SendEmail { get; set; }
    public AttachmentEntity? Attachment1 { get; set; }
    public AttachmentEntity? Attachment2 { get; set; }
    public CustomerEntity? CustomerEntity { get; set; }
    public PaymentFEEntity? PaymentEntity { get; set; }
    public List<GeneralAllowanceEntity>? GeneralAllowanceEntity { get; set; }
    public List<ItemElectronicEntity>? ItemElectronicEntity { get; set; }
    public string? Resolution { get; set; }
    public string? ResolutionText { get; set; }
    public string? HeadNote { get; set; }
    public string? FootNote { get; set; }
    public string? Notes { get; set; }
    public decimal? TotalBeforeTax { get; set; }
    public double? DiscountAmountByInvoice { get; set; }
    public string? DiscountType { get; set; }
    public double AllowanceTotal { get; set; }
    public double InvoiceBaseTotal { get; set; }
    public double InvoiceTaxExclusiveTotal { get; set; }
    public double InvoiceTaxInclusiveTotal { get; set; }
    public double TotalToPay { get; set; }
    public List<AllTaxTotalEntity>? AllTaxTotalEntity { get; set; }
    public List<AllHoldingsTaxTotalEntity>? AllHoldingsTaxTotalEntity { get; set; }

    public double FinalTotalToPay { get; set; }
};