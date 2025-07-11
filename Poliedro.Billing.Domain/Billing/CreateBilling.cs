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
    public OrderReferenceEntity? OrderReference { get; set; }
    public bool SendEmail { get; set; }
    public AttachmentEntity? Attachment1 { get; set; }
    public AttachmentEntity? Attachment2 { get; set; }
    public CustomerEntity? CustomerEntity { get; set; }
    public PaymentEntity? PaymentEntity { get; set; }
    public List<GeneralAllowanceEntity>? GeneralAllowanceEntity { get; set; }

    public List<ItemElectronicEntity>? ItemElectronicEntity { get; set; }
    public decimal Resolution { get; set; }
    public string? ResolutionText { get; set; }
    public string? HeadNote { get; set; }
    public string? FootNote { get; set; }
    public string? Notes { get; set; }
    public decimal AllowanceTotal { get; set; }
    public decimal InvoiceBaseTotal { get; set; }
    public decimal InvoiceTaxExclusiveTotal { get; set; }
    public decimal InvoiceTaxInclusiveTotal { get; set; }
    public decimal TotalToPay { get; set; }
    public List<AllTaxTotalEntity>? AllTaxTotalEntity { get; set; }
    public List<AllHoldingsTaxTotalEntity>? AllHoldingsTaxTotalEntity { get; set; }

    public decimal FinalTotalToPay { get; set; }
};