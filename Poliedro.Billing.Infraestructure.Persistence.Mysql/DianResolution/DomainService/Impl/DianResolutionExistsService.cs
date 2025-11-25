using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;

public class DianResolutionExistsService(DataBaseContext context) : IDianResolutionExistsService
{
    public async Task<bool> EntityExists(int resolutionId, CancellationToken cancellationToken)
    {
        return await context.DianResolution
            .AsNoTracking()
            .AnyAsync(c => c.ResolutionId == resolutionId, cancellationToken);
    }
}
