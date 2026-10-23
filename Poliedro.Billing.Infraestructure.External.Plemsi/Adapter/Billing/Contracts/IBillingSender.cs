using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.Responses;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
public interface IBillingSender
{
    Task<List<ApiResponseFERetailPos>> SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken);
}
