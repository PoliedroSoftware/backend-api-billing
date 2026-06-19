using Poliedro.Billing.Domain.CompanyProvider.Enums;

namespace Poliedro.Billing.Domain.FERetail.Ports
{
    public interface IInsertInvoiceFE
    {
       Task InsertInvoiceSucces(
       int invoice,
       string? cude,
       string? QRCode,
       string connectionString,
       ProviderType providerType,
       int ClientBillingElectronicId,
       string invoiceResolution);
    }
}
