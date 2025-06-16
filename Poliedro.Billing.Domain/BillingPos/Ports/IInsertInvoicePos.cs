using Poliedro.Billing.Domain.Client.Enums;

namespace Poliedro.Billing.Domain.BillingPos.Ports;

public interface IInsertInvoicePos
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
