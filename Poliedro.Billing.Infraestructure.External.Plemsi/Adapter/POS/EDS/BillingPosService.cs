using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;


namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;

public class BillingPosService() : IBillingService
{
    public Task<Result<ApiResponseBillingPos, Error>> CreateInvoicesPosAsync(IEnumerable<ClientEntity> Clients, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}



