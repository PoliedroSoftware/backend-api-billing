using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.Server.DomainService;

public interface IServerUpdateService
{
    Task<Result<VoidResult, Error>> UpdateAsync(ServerEntity serverEntity);
}
