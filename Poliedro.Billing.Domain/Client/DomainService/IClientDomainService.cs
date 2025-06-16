using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Domain.Client.DomainService;

public interface IClientDomainService
{
    Task<Result<VoidResult, Error>> CreateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> UpdateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken);
    Task<Result<IEnumerable<ClientEntity>, Error>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<ClientEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<ClientEntity, Error>> GetByIdAsync(string Apikey, CancellationToken cancellationToken);
    Task<Result<ClientEntity, Error>> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task<Result<VoidResult, Error>> DeleteAsync(int id, CancellationToken cancellationToken);
}
