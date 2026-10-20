using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;


namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;

public class ServerDomainService(
    IServerCreateService createService,
    IServerUpdateService updateService,
    IServerGetAllService getAllService,
    IServerGetByIdService getByIdService) : IServerDomainService
{
    public Task<Result<VoidResult, Error>> CreateAsync(ServerEntity serverEntity)
        => createService.CreateAsync(serverEntity);

    public Task<Result<VoidResult, Error>> UpdateAsync(ServerEntity serverEntity)
        => updateService.UpdateAsync(serverEntity);

    public Task<IEnumerable<ServerEntity>> GetAllAsync(PaginationParams paginationParams)
        => getAllService.GetAllAsync(paginationParams);

    public Task<Result<ServerEntity, Error>> GetByIdAsync(int id)
        => getByIdService.GetByIdAsync(id, CancellationToken.None);
}
