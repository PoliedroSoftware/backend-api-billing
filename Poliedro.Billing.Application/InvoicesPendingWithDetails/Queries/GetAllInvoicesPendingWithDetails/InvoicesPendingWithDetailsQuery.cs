using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Common.Dtos;
namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

public record InvoicesPendingWithDetailsQuery(
    string ApiKey,
    int Page = 1,
    int PageSize = 10)
    : IRequest<PagedResponseDto<CreateBillingDTO>>;
