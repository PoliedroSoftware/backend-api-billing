using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Server.Entities;
namespace Poliedro.Billing.Domain.InvoicePendingWithDetails.Ports;

public interface IInvoicesPendingWithDetailsStrategy
{
    Task<IEnumerable<object>> GetAllInvoicePendingWithDetails(
            ServerEntity server,
            ClientEntity clientItem,
            IDatabaseUtils databaseUtils,
            CancellationToken cancellationToken,
            string ApiKey
        );
}