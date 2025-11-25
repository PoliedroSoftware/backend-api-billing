using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Domain.Client.DomainService;

public interface IClientDeleteService
{
    Task<Result<VoidResult, Error>> DeleteAsync(int id, CancellationToken cancellationToken);
}
