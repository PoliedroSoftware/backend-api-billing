namespace Poliedro.Billing.Application.PendingInvoice.Dtos;

public record PedingInvoiceDto(
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
    string Created_by_name
    );

