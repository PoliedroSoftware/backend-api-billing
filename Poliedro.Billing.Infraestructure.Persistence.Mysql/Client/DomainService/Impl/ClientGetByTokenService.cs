using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;

public class ClientGetByTokenService(DataBaseContext context) : IClientGetByTokenService
{
    public async Task<Result<ClientEntity, Error>> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await context.ClientBillingElectronic
            .Include(c => c.Server)
            .FirstAsync(c => c.ApiKey == token, cancellationToken);
    }
}
