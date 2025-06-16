using System.Collections.Generic;

namespace Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;

public record InvoiceElectronicDto(
    int Id,
    string? Identication,
    string? ContactName,
    string? Email,
    string? Mobile,
    string? City,
    string? State,
    string? Country,
    string? Invoice,
    string? PaymentStatus,
    DateTime? TransactionDate,
    int AllowanceTotal,
    decimal InvoiceBaseTotal,
    decimal InvoiceTaxExclusiveTotal,
    decimal InvoiceTaxInclusiveTotal,
    decimal TotalToPay,
    string? CreatedByName,

    List<InvoiceWithDetailElectronicDto> InvoiceElectronicDetails
);