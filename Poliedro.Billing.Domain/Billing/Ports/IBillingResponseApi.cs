using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingResponseApi
{
    Task IBillingResponseApi(List<ApiResponseFERetailPos> response, IEnumerable<CreateBilling> processedInvoices, CancellationToken cancellationToken);
}
