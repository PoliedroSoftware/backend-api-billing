using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Billing;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;

public class FERetailService : IFERetailService
{
    public Task<Result<BillingProviderResponse, Error>> CreateElectronicInvoicesAsync(IEnumerable<ClientEntity> clients, CancellationToken cancellationToken)
    {
        var error = Error.CreateInstance("NotImplemented", "FERetail service stub: operation not implemented", HttpStatusCode.NotImplemented);
        return Task.FromResult(Result<BillingProviderResponse, Error>.Failure(error));
    }
}
