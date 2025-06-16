
namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Dtos;

public record DetailsInvoicePOSPendingDto(
    int Id,
    string Description,
    decimal Code,
    decimal BaseQuantity,
    decimal InvoicedQuantity,
    decimal PriceAmount,
    decimal Subtotal
);
