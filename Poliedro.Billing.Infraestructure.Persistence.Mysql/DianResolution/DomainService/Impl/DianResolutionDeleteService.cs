using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.DianResolution.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;

public class DianResolutionDeleteService(DataBaseContext context) : IDianResolutionDeleteService
{
    public async Task<Result<VoidResult, Error>> DeleteAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
    {
        var entity = await context.DianResolution.FirstOrDefaultAsync(x => x.ResolutionId == dianResolutionEntity.ResolutionId, cancellationToken);
        if (entity == null)
            return DianResolutionErrorBuilder.DianResolutionNotFoundException(dianResolutionEntity.ResolutionId);

        context.DianResolution.Remove(entity);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        if (!result)
            return DianResolutionErrorBuilder.DianResolutionDeletionException(dianResolutionEntity.ResolutionId);

        return VoidResult.Instance;
    }
}
