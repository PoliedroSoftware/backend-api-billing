using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.BillingPos;

public class CreateBilling
{
    public int Id { get; set; }
    public decimal AllowanceTotal { get; set; }
    public decimal InvoiceBaseTotal { get; set; }
    public decimal InvoiceTaxExclusiveTotal { get; set; }
    public decimal InvoiceTaxInclusiveTotal { get; set; }
    public decimal TotalToPay { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? Time { get; set; }
    public string? Resolution { get; set; }
    public string? Prefix { get; set; }
    public string? Number { get; set; }
    public string? SendEmail { get; set; }
    public string? ResolutionType { get; set; }
    public string? Note { get; set; }
    public string? Identication { get; set; }
    public string? ContactName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Invoice { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TransactionDate { get; set; }
    public string? CreatedByName { get; set; }
    public decimal? Percent { get; set; }
    public decimal? InvoiceTax { get; set; }
    public int? SendDian { get; set; }
    public string? resolutionText { get; set; }
    public string? head_note { get; set; }
    public string? foot_note { get; set; }
    public string? notes { get; set; }
    public decimal? finalTotalToPay { get; set; }

    public OrderReferenceEntity?  orderReference { get; set; }

    public AttachmentEntity? attachment1 { get; set; }

    public AttachmentEntity? attachment2 { get; set; }

    public CustomerEntity? customerEntity { get; set; }

    public PaymentEntity? paymentEntity { get; set; }

    public GeneralAllowanceEntity? generalAllowanceEntity { get; set; }

    public ItemElectronicEntity? itemElectronicEntity { get; set; }

    public AllTaxTotalEntity? allTaxTotalEntity { get; set; }

    public AllHoldingsTaxTotalEntity? allHoldingsTaxTotalEntity { get; set; }

    public CustomSubtotalEntity? customSubtotalEntity { get; set; }   


};