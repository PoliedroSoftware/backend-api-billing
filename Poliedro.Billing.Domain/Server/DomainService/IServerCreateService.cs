using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.Server.DomainService;

public interface IServerCreateService
{
    Task<Result<VoidResult, Error>> CreateAsync(ServerEntity serverEntity);
}
