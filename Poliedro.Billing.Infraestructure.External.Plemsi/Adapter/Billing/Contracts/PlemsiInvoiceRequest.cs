using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
public class PlemsiInvoiceRequest
{
    public required DianResolutionEntity DianResolutionEntity { get; set; }

    public required CompanyProviderEntity CompanyProviderEntity { get; set; }

    public required IEnumerable<object> Invoices { get; init; }
}
