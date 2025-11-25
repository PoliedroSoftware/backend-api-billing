using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.Server.DomainService;

public interface IServerGetByIdService
{
    Task<Result<ServerEntity, Error>> GetByIdAsync(int id);
}
