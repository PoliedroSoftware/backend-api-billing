using MediatR;
using Poliedro.Billing.Application.GetInvoice.Dtos;

namespace Poliedro.Billing.Application.GetInvoice.Queries;

public record GetByIdGetInvoiceQuery(string cufe, string token) : IRequest<GetInvoiceDto>;