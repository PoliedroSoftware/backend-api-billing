using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.Client.Errors;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientBillingDomainService(DataBaseContext context) : IClientDomainService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken)
    {
        await context.ClientBillingElectronic.AddAsync(clientBillingElectronicEntity, cancellationToken);
        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return ClientBillingElectronicErrorBuilder.ClientBillingCreationException();

        return VoidResult.Instance;
    }

    public async Task<Result<VoidResult, Error>> UpdateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken)
    {
        if (!await EntityExists(clientBillingElectronicEntity.ClientBillingElectronicId, cancellationToken))
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(clientBillingElectronicEntity.ClientBillingElectronicId);

        context.ClientBillingElectronic.Update(clientBillingElectronicEntity);
        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return ClientBillingElectronicErrorBuilder.ClientBillingUpdateException(clientBillingElectronicEntity.ClientBillingElectronicId);

        return VoidResult.Instance;
    }

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

    public async Task<Result<ClientEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (!await EntityExists(id, cancellationToken))
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(id);

        return await context.ClientBillingElectronic
            .Include(c => c.DianResolution)
            .Include(c => c.Server)
            .FirstAsync(c => c.ClientBillingElectronicId == id);
    }

    public async Task<Result<ClientEntity, Error>> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
    
        return await context.ClientBillingElectronic
            .Include(c => c.Server)
            .FirstAsync(c => c.ApiKey == token);
    }



    public async Task<Result<VoidResult, Error>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await context.ClientBillingElectronic.FirstOrDefaultAsync(x => x.ClientBillingElectronicId == id);
        if (entity == null)
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(id);

        context.ClientBillingElectronic.Remove(entity);
        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return ClientBillingElectronicErrorBuilder.ClientBillingDeletionException(id);

        return VoidResult.Instance;
    }

    private async Task<bool> EntityExists(int id, CancellationToken cancellationToken)
    {
        return await context.ClientBillingElectronic
            .AsNoTracking()
            .AnyAsync(c => c.ClientBillingElectronicId == id, cancellationToken);
    }

    public async Task<Result<ClientEntity, Error>> GetByIdAsync(string Apikey, CancellationToken cancellationToken)
    {
        return await context.ClientBillingElectronic
            .Include(c => c.DianResolution)
            .Include(c => c.Server)
            .Where(c => c.Active == true)
            .FirstAsync(c => c.ApiKey == Apikey);
    }
}
