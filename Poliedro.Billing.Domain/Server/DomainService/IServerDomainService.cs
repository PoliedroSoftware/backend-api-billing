using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.Server.DomainService
{
    public interface IServerDomainService
    {
        Task<Result<VoidResult, Error>> CreateAsync(ServerEntity serverEntity);
        Task<Result<ServerEntity, Error>> GetByIdAsync(int id);
        Task<IEnumerable<ServerEntity>> GetAllAsync(PaginationParams paginationParams);
        Task<Result<VoidResult, Error>> UpdateAsync(ServerEntity ServerEntity);
    }
}
