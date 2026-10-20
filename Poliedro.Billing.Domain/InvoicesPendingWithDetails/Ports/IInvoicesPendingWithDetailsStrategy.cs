using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.Entities;
namespace Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

public interface IInvoicesPendingWithDetailsStrategy
{
    Task<IEnumerable<CreateBilling>> GetAllInvoicePendingWithDetails(
            ServerEntity server,
            DianResolutionEntity dianResolutionEntity,
            CancellationToken cancellationToken
        );
}