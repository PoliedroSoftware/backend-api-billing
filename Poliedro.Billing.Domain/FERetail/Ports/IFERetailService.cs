using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Billing;

namespace Poliedro.Billing.Domain.FERetail.Ports;

public interface IFERetailService
{

    Task<Result<BillingProviderResponse, Error>> CreateElectronicInvoicesAsync(
            IEnumerable<ClientEntity> clients,
            CancellationToken cancellationToken);
}



