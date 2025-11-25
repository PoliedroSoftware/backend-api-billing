using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;

public class ServerGetAllService(DataBaseContext context) : IServerGetAllService
{
    public async Task<IEnumerable<ServerEntity>> GetAllAsync(PaginationParams paginationParams)
    {
        return await context.Server
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();
    }
}
