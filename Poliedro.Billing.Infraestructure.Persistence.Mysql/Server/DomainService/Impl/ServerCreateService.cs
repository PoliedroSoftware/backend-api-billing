using Poliedro.Billing.Application.Server.Errors;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;

public class ServerCreateService(DataBaseContext context) : IServerCreateService
{
    public async Task<Result<VoidResult, Error>> CreateAsync(ServerEntity serverEntity)
    {
        await context.Server.AddAsync(serverEntity);
        var result = await context.SaveChangesAsync() > 0;
        if (!result)
            return ServerErrorBuilder.ServerCreationException();

        return VoidResult.Instance;
    }
}
