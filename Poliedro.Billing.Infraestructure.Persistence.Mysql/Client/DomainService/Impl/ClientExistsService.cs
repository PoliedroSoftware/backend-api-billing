using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientExistsService(DataBaseContext context) : IClientExistsService
{
    public async Task<bool> EntityExists(int id, CancellationToken cancellationToken)
    {
        return await context.ClientBillingElectronic
            .AsNoTracking()
            .AnyAsync(c => c.ClientBillingElectronicId == id, cancellationToken);
    }
}
