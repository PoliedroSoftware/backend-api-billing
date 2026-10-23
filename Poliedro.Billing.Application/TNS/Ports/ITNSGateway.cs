using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.TNS.Contracts.Models;
using Poliedro.Billing.Application.TNS.Contracts.Responses;

namespace Poliedro.Billing.Application.TNS.Ports
{
    public interface ITNSGateway
    {
        Task<Result<ApiResponseTNS, Error>> CreateAsync(List<Order> request, string codigosucursal, string  token, CancellationToken cancellationToken);
    }
}
