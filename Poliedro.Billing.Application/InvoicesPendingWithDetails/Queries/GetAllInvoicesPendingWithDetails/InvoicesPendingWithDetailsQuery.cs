using MediatR;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Dtos;
namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;
public record InvoicesPendingWithDetailsQuery(
    string? ApiKey
    ): IRequest<IEnumerable<object>>;