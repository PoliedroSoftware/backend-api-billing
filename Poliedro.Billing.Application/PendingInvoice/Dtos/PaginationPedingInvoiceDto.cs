namespace Poliedro.Billing.Application.PendingInvoice.Dtos;

public record PaginationPedingInvoiceDto<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    string OrderBy
);
