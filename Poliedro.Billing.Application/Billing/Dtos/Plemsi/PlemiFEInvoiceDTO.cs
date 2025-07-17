using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;

public record PlemiFEInvoiceDTO
(
    string date,

    string time,

    string prefix,

    int number,

    OrderReferenceDTO orderReferenceDto,

     bool send_email,

    AttachmentDTO attachment1,

    AttachmentDTO attachment2,

    CustomerBillingDTO customer,

    PaymentDTO payment,

    List<GeneralAllowanceDTO>? generalAllowances,

    List<ItemElectronicDTO> items,

     string resolution,

     string? resolutionText,

    string? head_note,

     string? foot_note,

    string? notes,

    double allowanceTotal,

    double invoiceBaseTotal,

    double invoiceTaxExclusiveTotal,

     double invoiceTaxInclusiveTotal,

     double totalToPay,

    List<AllTaxTotalDTO>? allTaxTotals,

     List<AllHoldingsTaxTotalDTO>? allHoldingsTaxTotals,

     List<CustomSubtotalDTO>? customSubtotals,

     double finalTotalToPay

);
