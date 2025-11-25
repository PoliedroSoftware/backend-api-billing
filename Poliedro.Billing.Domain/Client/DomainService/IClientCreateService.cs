using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Domain.Client.DomainService;

public interface IClientCreateService
{
    Task<Result<VoidResult, Error>> CreateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken);
}
