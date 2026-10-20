using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Domain.Client.DomainService;

public interface IClientGetByIdService
{
    Task<Result<ClientEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
}
