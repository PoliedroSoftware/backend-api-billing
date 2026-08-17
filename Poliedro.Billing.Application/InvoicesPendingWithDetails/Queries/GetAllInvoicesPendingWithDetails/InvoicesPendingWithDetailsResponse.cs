using Poliedro.Billing.Application.Billing.Dtos;

namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

public record InvoicesPendingWithDetailsResponse(
    bool Success,
    int Count,
    List<CreateBillingDTO> Data);