using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.TNS.Contracts.Models;
using Poliedro.Billing.Application.TNS.Contracts.Responses;

namespace Poliedro.Billing.Application.Tns.Commands
{
    public record CreateSalesTNSCommand(
        List<Order> Orders,
        string codigosucursal,
        string? token
    ) : IRequest<Result<ApiResponseTNS, Error>>;
}
