using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Application.Server.Errors;
using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;

public class ServerDomainService(DataBaseContext context) : IServerDomainService
{
    public async Task<IEnumerable<ServerEntity>> GetAllAsync(PaginationParams paginationParams)
    {
        var totalRows = await context.Server.CountAsync();

      return await context.Server
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();
    }
    public async Task<Result<ServerEntity, Error>> GetByIdAsync(int id)
    {
        if (!await EntityExists(id))
            return ServerErrorBuilder.ServerNotFoundException(id);

        return await context.Server
            .Include(c => c.clientsBillingElectronic)
            .FirstAsync(c => c.ServerId == id);
    }

    public async Task<Result<VoidResult, Error>> UpdateAsync(ServerEntity serverEntity)
    {
        if (!await EntityExists(serverEntity.ServerId))
            return ServerErrorBuilder.ServerNotFoundException(serverEntity.ServerId);

        context.Server.Update(serverEntity);

        if (await context.SaveChangesAsync() <= 0)
            return ServerErrorBuilder.ServerUpdateException();

        return VoidResult.Instance;
    }

    public async Task<Result<VoidResult, Error>> CreateAsync(ServerEntity serverEntity)
    {
        await context.Server.AddAsync(serverEntity);
        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return ServerErrorBuilder.ServerCreationException();

        return VoidResult.Instance;
    }

    private async Task<bool> EntityExists(int id)
    {
        return await context.Server
            .AsNoTracking()
            .AnyAsync(c => c.ServerId == id);
    }
}
