
namespace Poliedro.Billing.Application.Billing.Dtos;

public record CreateBillingDTO
    (
        DateTime Date,
        DateTime Time,
        string? SendToEmail,
        SoftwareManufacturerDTO? SoftwareManufacturer,
        PayPointInfoDTO? PayPointInfo,
        string Prefix,
        int Number,
        OrderReferenceDTO? OrderReference,
        bool SendEmail,
        AttachmentDTO? Attachment1,
        AttachmentDTO? Attachment2,
        CustomerBillingDTO? CustomerEntity,
        PayPointInfoDTO? PayPointInfoEntity,
        PaymentDTO? PaymentEntity,
        List<GeneralAllowanceDTO>? GeneralAllowanceEntity,
        List<ItemElectronicDTO>? ItemElectronicEntity,
        int Resolution,
        string? HeadNote,
        string? FootNote,
        string? Notes,
        decimal AllowanceTotal,
        decimal InvoiceBaseTotal,
        decimal InvoiceTaxExclusiveTotal,
        decimal InvoiceTaxInclusiveTotal,
        decimal TotalToPay,
        List<AllTaxTotalDTO>? AllTaxTotalEntity,
        List<AllHoldingsTaxTotalDTO>? AllHoldingsTaxTotalEntity,
        decimal FinalTotalToPay
    );