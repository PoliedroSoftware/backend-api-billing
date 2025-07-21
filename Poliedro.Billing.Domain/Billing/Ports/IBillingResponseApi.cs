using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingResponseApi
{
    Task IBillingResponseApi(ApiResponseFERetailPos response, IEnumerable<object> processedInvoices, CancellationToken cancellationToken);
}
