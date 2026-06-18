using MediatR;
using Poliedro.Billing.Application.LastInvoiceNumber.Dtos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.LastInvoiceNumber.Queries.GetLastInvoiceNumber;

public record GetLastInvoiceNumberQuery(int ApiKey) : IRequest<Result<LastInvoiceNumberDto, Error>>;
