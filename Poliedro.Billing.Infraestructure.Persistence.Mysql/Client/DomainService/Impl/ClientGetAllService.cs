using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.Client.Errors;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientGetAllService(DataBaseContext context) : IClientGetAllService
{
    public async Task<Result<IEnumerable<ClientEntity>, Error>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await context.ClientBillingElectronic
            .Include(c => c.DianResolution)
            .Include(c => c.Server)
            .Where(c => c.Active == true)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
            return ClientBillingElectronicErrorBuilder.NoClientBillingRecordsFoundException();

        return entities;
    }
}
