using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Domain.Resolution.DomainService;

public interface IDianResolutionDomainService
{
    Task<Result<VoidResult, Error>> CreateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancelationToken);
    Task<Result<VoidResult, Error>> UpdateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancelationToken);
    Task<Result<VoidResult, Error>> DeleteAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancelationToken);
    Task<Result<IEnumerable<DianResolutionEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<DianResolutionEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
}
