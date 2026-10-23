namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IInsertInvoicePos
{
    Task InsertInvoiceSucces(
           int invoice,
           string? cude,
           string? QRCode,
           string connectionString,
           int providerId,
           int ClientBillingElectronicId,
           string invoiceResolution);
}
