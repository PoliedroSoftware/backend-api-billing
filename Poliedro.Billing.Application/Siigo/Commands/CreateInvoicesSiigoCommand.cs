using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.Siigo.Contracts.Responses;

namespace Poliedro.Billing.Application.Siigo.Commands
{
    public record CreateInvoicesSiigoCommand(
        List<Poliedro.Billing.Application.Siigo.Contracts.Models.Invoice> Invoices,
        string? token
    ) : IRequest<Result<ApiResponseSiigo, Error>>;
}
