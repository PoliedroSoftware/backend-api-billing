using Poliedro.Billing.Application.InvoicePendingWithDetails.Dtos;

namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Dtos;

public record InvoiceFEPendingDto
    (
    int Id,
    string Identication,
    string Contact_name,
    string Name,
    string Email,
    string Mobile,
    string City,
    string State,
    string Country,
    string Invoice,
    string Payment_status,
    string Transaction_date,
    double AllowanceTotal,
    double InvoiceBaseTotal,
    double InvoiceTaxExclusiveTotal,
    double InvoiceTaxInclusiveTotal,
    double TotalToPay,
    int SendDian,
    string Created_by_name,

    List<DetailsInvoiceFEPendingDto> DetailsInvoicePendings
    );