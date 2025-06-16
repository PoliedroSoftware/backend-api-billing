using System;
namespace Poliedro.Billing.Application.SuccessInvoice.Dtos;
public record SuccessInvoiceDto(
    string _Id,
    string Company,
    string CreatedBy,
    string State,
    string? Preview,
    string Resolution_Number ,
    string? Prefix,
    string Number ,
    int Type_Document_Id,
    string Date,
    string Time,
    CustomerDto Customer,
    OrderReferenceDto? Order_Reference,
    PaymentFormDto? Payment_Form,
    LegalMonetaryTotalsDto Legal_Monetary_Totals,
    string? EmailDeliveryStatus,
    bool SendEmailNotification,
    string? Consecutive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? Cude
);
