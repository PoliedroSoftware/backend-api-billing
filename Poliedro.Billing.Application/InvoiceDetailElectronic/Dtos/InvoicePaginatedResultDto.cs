
namespace Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;
public record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
);