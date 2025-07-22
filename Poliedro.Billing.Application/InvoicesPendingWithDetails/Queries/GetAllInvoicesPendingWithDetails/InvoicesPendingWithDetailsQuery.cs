using MediatR;
using Poliedro.Billing.Domain.Billing;
namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;
public record InvoicesPendingWithDetailsQuery(
    string? ApiKey
    ): IRequest<IEnumerable<CreateBilling>>;