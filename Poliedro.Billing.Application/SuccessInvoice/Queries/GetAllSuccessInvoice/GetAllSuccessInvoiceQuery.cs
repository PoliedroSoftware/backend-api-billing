using MediatR;
using Poliedro.Billing.Application.SuccessInvoice.Dtos;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;

namespace Poliedro.Billing.Application.SuccessInvoice.Queries.GetAllSuccessInvoice;

public record GetAllSuccessInvoiceQuery(
    SuccessInvoiceQueryParameters Parameters
) : IRequest<IEnumerable<SuccessInvoiceDto>>;