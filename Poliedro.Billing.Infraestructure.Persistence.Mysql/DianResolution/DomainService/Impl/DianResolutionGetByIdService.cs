using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;

public class DianResolutionGetByIdService(DataBaseContext context) : IDianResolutionGetByIdService
{
    public async Task<Result<DianResolutionEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var dianResolution = await context.DianResolution
            .FindAsync([id], cancellationToken);
        
        if (dianResolution != null)
        {
            return dianResolution;
        }
        
        return null!;
    }
}
