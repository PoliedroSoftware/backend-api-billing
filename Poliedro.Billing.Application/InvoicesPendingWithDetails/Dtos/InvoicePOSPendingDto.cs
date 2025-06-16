
namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Dtos;

public record InvoicePOSPendingDto
    (
        int Id,
        DateTime Date,
        TimeSpan Time,
        string Resolution,
        string Prefix,
        string Number,
        string ResolutionType,
        string Note,
        decimal AllowanceTotal,
        decimal InvoiceBaseTotal,
        decimal InvoiceTaxExclusiveTotal,
        decimal InvoiceTaxInclusiveTotal,
        decimal TotalToPay,
        List<DetailsInvoicePOSPendingDto> DetailsInvoicePendings
    );
