using Poliedro.Billing.Application.Client.Errors;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientUpdateService(DataBaseContext context, IClientExistsService existsService) : IClientUpdateService
{
    public async Task<Result<VoidResult, Error>> UpdateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken)
    {
        if (!await existsService.EntityExists(clientBillingElectronicEntity.CompanyId, cancellationToken))
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(clientBillingElectronicEntity.CompanyId);

        context.ClientBillingElectronic.Update(clientBillingElectronicEntity);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        if (!result)
            return ClientBillingElectronicErrorBuilder.ClientBillingUpdateException(clientBillingElectronicEntity.CompanyId);

        return VoidResult.Instance;
    }
}
