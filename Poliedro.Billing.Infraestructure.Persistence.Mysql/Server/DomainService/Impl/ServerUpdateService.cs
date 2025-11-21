using Poliedro.Billing.Application.Server.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;

public class ServerUpdateService(DataBaseContext context, IServerExistsService existsService) : IServerUpdateService
{
    public async Task<Result<VoidResult, Error>> UpdateAsync(ServerEntity serverEntity)
    {
        if (!await existsService.EntityExists(serverEntity.ServerId))
            return ServerErrorBuilder.ServerNotFoundException(serverEntity.ServerId);

        context.Server.Update(serverEntity);

        if (await context.SaveChangesAsync() <= 0)
            return ServerErrorBuilder.ServerUpdateException();

        return VoidResult.Instance;
    }
}
