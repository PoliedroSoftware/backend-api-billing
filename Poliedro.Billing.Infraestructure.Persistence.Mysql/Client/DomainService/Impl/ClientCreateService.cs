using Poliedro.Billing.Application.Client.Errors;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientCreateService(DataBaseContext context) : IClientCreateService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken)
    {
        await context.ClientBillingElectronic.AddAsync(clientBillingElectronicEntity, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        if (!result)
            return ClientBillingElectronicErrorBuilder.ClientBillingCreationException();

        return VoidResult.Instance;
    }
}
