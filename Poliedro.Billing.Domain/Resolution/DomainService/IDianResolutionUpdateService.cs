using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Domain.Resolution.DomainService;

public interface IDianResolutionUpdateService
{
    Task<Result<VoidResult, Error>> UpdateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken);
}
