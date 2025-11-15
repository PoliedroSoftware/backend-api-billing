using Poliedro.Billing.Application.Billing.Dtos;

namespace Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;
public record  CreateCreditNoteInputDTO
    (
        string? SendToEmail,
        DiscrepancyDTO? Discrepancy,
        SoftwareManufacturerDTO? SoftwareManufacturer,
        PayPointInfoDTO? PayPointInfo,
        string Number,
        string? Prefix,
        DateTime? TransactionDate,
        OrderReferenceDTO? OrderReference,
        bool? SendEmail,
        CustomerBillingDTO? CustomerEntity,
        PaymentDTO? PaymentEntity,
        List<GeneralAllowanceDTO>? GeneralAllowanceEntity,
        List<ItemElectronicDTO>? ItemElectronicEntity,
        string? Resolution,
        string? ResolutionText,
        string? HeadNote,
        string? FootNote,
        string? Notes,
        decimal? TotalBeforeTax,
        decimal? DiscountAmountByInvoice,
        string? DiscountType,
        double AllowanceTotal,
        double InvoiceBaseTotal,
        double InvoiceTaxExclusiveTotal,
        double InvoiceTaxInclusiveTotal,
        double TotalToPay,
        List<AllTaxTotalDTO>? AllTaxTotalEntity,
        List<AllHoldingsTaxTotalDTO>? AllHoldingsTaxTotalEntity
    );
