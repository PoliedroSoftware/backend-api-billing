using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Domain.Client.DomainService;

public interface IClientGetAllService
{
    Task<Result<IEnumerable<ClientEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
}
