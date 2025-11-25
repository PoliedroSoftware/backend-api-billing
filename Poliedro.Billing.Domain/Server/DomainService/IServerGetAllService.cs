using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.Server.DomainService;

public interface IServerGetAllService
{
    Task<IEnumerable<ServerEntity>> GetAllAsync(PaginationParams paginationParams);
}
