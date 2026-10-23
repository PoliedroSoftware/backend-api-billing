using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.Siigo.Contracts.Responses;
using Poliedro.Billing.Application.Siigo.Contracts.Models;

namespace Poliedro.Billing.Application.Siigo.Ports
{
    public interface ISiigoGateway
    {
        Task<Result<ApiResponseSiigo, Error>> CreateAndSendDianAsync(List<Invoice> request, string token, CancellationToken cancellationToken);
    }
}
