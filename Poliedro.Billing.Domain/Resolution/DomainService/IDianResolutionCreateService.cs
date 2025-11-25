using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Domain.Resolution.DomainService;

public interface IDianResolutionCreateService
{
    Task<Result<VoidResult, Error>> CreateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken);
}
