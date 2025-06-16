namespace Poliedro.Billing.Domain.FERetail.Entity;

public class FERetailEntity
{
    public int Id { get; set; }
    public required string date { get; set; }
    public required string time { get; set; }
    public required string prefix { get; set; }
    public int number { get; set; }
    public required SoftwareManufacturerEntity SoftwareManufacturer { get; set; }
    public required OrderReferenceEntity orderReference { get; set; }
    public required bool send_email { get; set; }
    public  AttachmentEntity? attachment1 { get; set; }
    public  AttachmentEntity? Attachment2 { get; set; }
    public required CustomerEntity customer { get; set; }
    public required PaymentEntity payment { get; set; }
    public List<GeneralAllowanceEntity>? generalAllowances { get; set; }
    public required List<ItemEntity> items { get; set; }
    public required string resolution { get; set; }
    public string? resolutionText { get; set; }
    public string? head_note { get; set; }
    public string? foot_note { get; set; }
    public string? notes { get; set; }
    public decimal? allowanceTotal { get; set; }
    public required decimal invoiceBaseTotal { get; set; }
    public required decimal invoiceTaxExclusiveTotal { get; set; }
    public required decimal invoiceTaxInclusiveTotal { get; set; }
    public required decimal totalToPay { get; set; }
    public List<AllTaxTotalEntity>? allTaxTotals { get; set; }
    public List<AllHoldingsTaxTotalEntity>? allHoldingsTaxTotals { get; set; }
    public List<CustomSubtotalEntity>? customSubtotals { get; set; }
    public required decimal finalTotalToPay { get; set; }

    

}

