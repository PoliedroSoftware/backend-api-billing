using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.Server.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;

public class ServerGetByIdService(DataBaseContext context, IServerExistsService existsService) : IServerGetByIdService
{
    public async Task<Result<ServerEntity, Error>> GetByIdAsync(int id)
    {
        if (!await existsService.EntityExists(id))
            return ServerErrorBuilder.ServerNotFoundException(id);

        return await context.Server
            .Include(c => c.clientsBillingElectronic)
            .FirstAsync(c => c.ServerId == id);
    }
}
