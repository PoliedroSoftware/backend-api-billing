using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.Client.Errors;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientGetByIdService(DataBaseContext context, IClientExistsService existsService) : IClientGetByIdService
{
    public async Task<Result<ClientEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (!await existsService.EntityExists(id, cancellationToken))
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(id);

        return await context.ClientBillingElectronic
            .Include(c => c.DianResolution)
            .Include(c => c.Server)
            .FirstAsync(c => c.ClientBillingElectronicId == id, cancellationToken);
    }

    public async Task<Result<ClientEntity, Error>> GetByIdAsync(string apiKey, CancellationToken cancellationToken)
    {
        return await context.ClientBillingElectronic
            .Include(c => c.DianResolution)
            .Include(c => c.Server)
            .Where(c => c.Active == true)
            .FirstAsync(c => c.ApiKey == apiKey, cancellationToken);
    }

    public async Task<Result<ClientEntity, Error>> GetByIdAsync(int clientId, int providerType, CancellationToken cancellationToken)
    {
        // New implementation: query out_client_client table joined with client_billing_electronic
        // Filter by client id and provider type (ProviderType enum value)

        // Assuming there is a DbSet mapped for the out_client_client table. If not, perform a raw SQL query.

        // We'll use FromSqlRaw to join out_client_client with client_billing_electronic
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
}
