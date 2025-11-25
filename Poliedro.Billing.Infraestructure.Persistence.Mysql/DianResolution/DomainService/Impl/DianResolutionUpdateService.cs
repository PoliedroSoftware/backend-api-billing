using Poliedro.Billing.Application.DianResolution.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;

public class DianResolutionUpdateService(DataBaseContext context, IDianResolutionExistsService existsService) : IDianResolutionUpdateService
{
    public async Task<Result<VoidResult, Error>> UpdateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
    {
        if (!await existsService.EntityExists(dianResolutionEntity.ResolutionId, cancellationToken))
            return DianResolutionErrorBuilder.DianResolutionUpdateException(dianResolutionEntity.ResolutionId);

        context.DianResolution.Update(dianResolutionEntity);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        if (!result)
            return DianResolutionErrorBuilder.DianResolutionUpdateException(dianResolutionEntity.ResolutionId);

        return VoidResult.Instance;
    }
}
