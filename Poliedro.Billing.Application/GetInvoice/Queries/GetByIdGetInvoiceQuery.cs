using MediatR;
using Poliedro.Billing.Domain.GetInvoice.Entities;

namespace Poliedro.Billing.Application.GetInvoice.Queries.GetByIdGetInvoice;

public record GetByIdGetInvoiceQuery(string cufe, string token) : IRequest<GetInvoiceDto>;