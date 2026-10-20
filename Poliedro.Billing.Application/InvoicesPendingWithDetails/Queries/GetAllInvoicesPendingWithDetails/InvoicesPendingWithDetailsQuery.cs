using MediatR;
namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;
public record InvoicesPendingWithDetailsQuery(int Id) : IRequest<InvoicesPendingWithDetailsResponse?>;