using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;
public record InvoicesPendingWithDetailsQuery(int Id): IRequest<IEnumerable<CreateBillingDTO>>;