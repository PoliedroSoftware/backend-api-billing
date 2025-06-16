namespace Poliedro.Billing.Application.GetInvoice.Dtos
{
    public record PaginationGetInvoiceDto<T>
    (
        List<T> Data,
        int TotalPages,
        int TotalRows
    );
}
