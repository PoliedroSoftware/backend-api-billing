using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Domain.Client.DomainService;

public interface IClientGetByTokenService
{
    Task<Result<ClientEntity, Error>> GetByTokenAsync(string token, CancellationToken cancellationToken);
}
