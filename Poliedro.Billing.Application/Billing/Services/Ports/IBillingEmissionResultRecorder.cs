using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Resolution.Entities;
namespace Poliedro.Billing.Application.Billing.Services.Ports;

/// <summary>Application boundary for recording provider-confirmed invoice emissions.</summary>
public interface IBillingEmissionResultRecorder
{
    Task RecordAsync(IReadOnlyList<BillingProviderResponse> response,
        IEnumerable<CreateBilling> processedInvoices,
        DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        CancellationToken cancellationToken);
}

/// <summary>Stores the accepted invoice reference in the billing database.</summary>
public interface IInvoiceEmissionStore
{
    Task RecordSuccessAsync(int invoiceNumber, string cude, string qrCode, string connectionString,
        int providerId, int clientId, string invoiceResolution, CancellationToken cancellationToken);
}
