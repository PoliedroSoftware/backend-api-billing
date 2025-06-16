using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.DianResolution.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;

public class DianResolutionDomainService(DataBaseContext context) : IDianResolutionDomainService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
    {
        await context.DianResolution.AddAsync(dianResolutionEntity);
        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return DianResolutionErrorBuilder.DianResolutionCreationException();

        return VoidResult.Instance;
    }

    public async Task<Result<VoidResult, Error>> UpdateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
    {
        if (!await EntityExists(dianResolutionEntity.ResolutionId))
            return DianResolutionErrorBuilder.DianResolutionUpdateException(dianResolutionEntity.ResolutionId);

        context.DianResolution.Update(dianResolutionEntity);
        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return DianResolutionErrorBuilder.DianResolutionUpdateException(dianResolutionEntity.ResolutionId);

        return VoidResult.Instance;
    }

    public async Task<Result<VoidResult, Error>> DeleteAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
    {
        var entity = await context.DianResolution.FirstOrDefaultAsync(x => x.ResolutionId == dianResolutionEntity.ResolutionId);
        if (entity == null)
            return DianResolutionErrorBuilder.DianResolutionNotFoundException(dianResolutionEntity.ResolutionId);

        context.DianResolution.Remove(entity);
        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return DianResolutionErrorBuilder.DianResolutionDeletionException(dianResolutionEntity.ResolutionId);

        return VoidResult.Instance;
    }

    public async Task<Result<IEnumerable<DianResolutionEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await context.DianResolution
        .Include(c => c.clientsBillingElectronic)
        .Where(c => c.Active == true)
        .ToListAsync(cancellationToken);

        if (entities.Count == 0)
            return DianResolutionErrorBuilder.NoDianResolutionFoundException();

        return entities;
    }

    public async Task<Result<DianResolutionEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var dianResolution = await context.DianResolution
            .FindAsync(id, cancellationToken);
        if (dianResolution != null)
        {
            return dianResolution;
        }
        return null;
    }

    private async Task<bool> EntityExists(int ResolutionId)
    {
        return await context.DianResolution
            .AsNoTracking()
            .AnyAsync(c => c.ResolutionId == ResolutionId);
    }


}
