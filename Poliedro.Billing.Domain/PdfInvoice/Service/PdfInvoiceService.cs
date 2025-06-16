namespace Poliedro.Billing.Domain.PdfInvoice.Service;
public interface IPdfInvoiceService
{
    Task<byte[]> GetPdfBytes(string id, string bearerToken);
}

