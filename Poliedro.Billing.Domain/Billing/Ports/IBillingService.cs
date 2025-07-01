using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Domain.BillingPos.Ports;

public interface IBillingService
{
    Task<Result<ApiResponseBillingPos, Error>> CreateInvoicesPosAsync(
        IEnumerable<ClientEntity> Clients,
        CancellationToken cancellationToken);
}
