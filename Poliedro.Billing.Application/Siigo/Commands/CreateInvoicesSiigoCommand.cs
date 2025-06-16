using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Siigo.ResponseModels;

namespace Poliedro.Billing.Application.Siigo.Commands
{
    public record CreateInvoicesSiigoCommand(
        List<Poliedro.Billing.Domain.Siigo.Models.Invoice> Invoices,
        string? token
    ) : IRequest<Result<ApiResponseSiigo, Error>>;
}
