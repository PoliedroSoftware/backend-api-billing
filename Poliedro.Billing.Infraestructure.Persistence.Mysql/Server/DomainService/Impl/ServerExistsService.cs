using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;

public class ServerExistsService(DataBaseContext context) : IServerExistsService
{
    public async Task<bool> EntityExists(int id)
    {
        return await context.Server
            .AsNoTracking()
            .AnyAsync(c => c.ServerId == id);
    }
}
