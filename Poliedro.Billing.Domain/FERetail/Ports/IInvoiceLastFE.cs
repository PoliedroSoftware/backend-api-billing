using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.FERetail.Ports
{
    public interface IInvoiceLastFE
    {
        Task<int> GetInvoiceLastAsync(string connectionString, ClientEntity client, CancellationToken cancellationToken);
    }
}
