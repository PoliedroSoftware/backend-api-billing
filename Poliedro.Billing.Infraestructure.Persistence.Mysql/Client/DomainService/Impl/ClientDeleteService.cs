using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.Client.Errors;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientDeleteService(DataBaseContext context) : IClientDeleteService
{
    public async Task<Result<VoidResult, Error>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await context.ClientBillingElectronic.FirstOrDefaultAsync(x => x.ClientBillingElectronicId == id, cancellationToken);
        if (entity == null)
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(id);

        context.ClientBillingElectronic.Remove(entity);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        if (!result)
            return ClientBillingElectronicErrorBuilder.ClientBillingDeletionException(id);

        return VoidResult.Instance;
    }
}
