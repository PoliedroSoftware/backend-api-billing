using Poliedro.Billing.Application.DianResolution.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;

public class DianResolutionCreateService(DataBaseContext context) : IDianResolutionCreateService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(DianResolutionEntity dianResolutionEntity, CancellationToken cancellationToken)
    {
        await context.DianResolution.AddAsync(dianResolutionEntity, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        if (!result)
            return DianResolutionErrorBuilder.DianResolutionCreationException();

        return VoidResult.Instance;
    }
}
