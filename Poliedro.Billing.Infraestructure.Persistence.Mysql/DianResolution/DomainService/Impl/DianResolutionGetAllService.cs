using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.DianResolution.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;

public class DianResolutionGetAllService(DataBaseContext context) : IDianResolutionGetAllService
{
    public async Task<Result<IEnumerable<DianResolutionEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await context.DianResolution
            .Where(c => c.Active == true)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
            return DianResolutionErrorBuilder.NoDianResolutionFoundException();

        return entities;
    }
}
