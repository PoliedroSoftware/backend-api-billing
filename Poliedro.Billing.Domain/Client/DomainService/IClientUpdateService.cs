using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Domain.Client.DomainService;

public interface IClientUpdateService
{
    Task<Result<VoidResult, Error>> UpdateAsync(ClientEntity clientBillingElectronicEntity, CancellationToken cancellationToken);
}
