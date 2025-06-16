using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.TNS.Models;
using Poliedro.Billing.Domain.TNS.ResponseModels;

namespace Poliedro.Billing.Application.Tns.Commands
{
    public record CreateSalesTNSCommand(
        List<Order> Orders,
        string codigosucursal,
        string? token
    ) : IRequest<Result<ApiResponseTNS, Error>>;
}
