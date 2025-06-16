using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.TNS.Models;
using Poliedro.Billing.Domain.TNS.ResponseModels;

namespace Poliedro.Billing.Domain.TNS.DomainServices
{
    public interface ITNSDomainService
    {
        Task<Result<ApiResponseTNS, Error>> CreateAsync(List<Order> request, string codigosucursal, string  token, CancellationToken cancellationToken);
    }
}
