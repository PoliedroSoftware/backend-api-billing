using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Siigo.ResponseModels;

namespace Poliedro.Billing.Domain.Siigo.DomainServices
{
    public interface ISiigoDomainService
    {
        Task<Result<ApiResponseSiigo, Error>> CreateAndSendDianAsync(List<Models.Invoice> request, string token, CancellationToken cancellationToken);
    }
}
