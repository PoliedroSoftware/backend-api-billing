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
            .Include(c => c.DianResolutionCreditNote)
            .FirstAsync(c => c.ClientBillingElectronicId == id);
    }

    public async Task<Result<ClientEntity, Error>> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
    
        return await context.ClientBillingElectronic
            .Include(c => c.Server)
            .FirstAsync(c => c.ApiKey == token);
    }


    public async Task<Result<ClientEntity, Error>> GetByIdAsync(int clientId, int providerType, CancellationToken cancellationToken)
    {
        // Query client_billing_electronic joined with out_client_client to find client by external client id and provider
        var sql = @"SELECT c.* FROM client_billing_electronic c
                        INNER JOIN out_client_client oc ON oc.client_billing_electronic_id = c.client_billing_electronic_id
                        WHERE oc.client_id = {0} AND oc.provider = {1} AND c.active = 1 LIMIT 1";

        var client = await context.ClientBillingElectronic
            .FromSqlRaw(sql, clientId, providerType)
            .Include(c => c.DianResolution)
            .Include(c => c.Server)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (client is null)
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(clientId);

        return client;
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
            .Include(c => c.DianResolutionCreditNote)
            .Where(c => c.Active == true)
            .FirstAsync(c => c.ApiKey == Apikey);
    }
}
