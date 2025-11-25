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
}
