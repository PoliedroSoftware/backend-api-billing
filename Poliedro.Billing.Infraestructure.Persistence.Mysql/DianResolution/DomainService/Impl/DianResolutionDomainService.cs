using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;

public class DianResolutionDomainService(
    IDianResolutionCreateService createService,
    IDianResolutionUpdateService updateService,
    IDianResolutionDeleteService deleteService,
    IDianResolutionGetAllService getAllService,
    IDianResolutionGetByIdService getByIdService) : IDianResolutionDomainService
{
    public Task<Result<VoidResult, Error>> CreateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
        => createService.CreateAsync(dianResolutionEntity, cancellationToken);

    public Task<Result<VoidResult, Error>> UpdateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
        => updateService.UpdateAsync(dianResolutionEntity, cancellationToken);

    public Task<Result<VoidResult, Error>> DeleteAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
        => deleteService.DeleteAsync(dianResolutionEntity, cancellationToken);

    public Task<Result<IEnumerable<DianResolutionEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
        => getAllService.GetAllAsync(cancellationToken);

    public Task<Result<DianResolutionEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
        => getByIdService.GetByIdAsync(id, cancellationToken);
}
