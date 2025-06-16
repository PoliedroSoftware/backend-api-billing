using Poliedro.Billing.Domain.InvoicePos.Entities;

namespace Poliedro.Billing.Domain.InvoicePos.Ports;

public interface IInvoicePosRepository
{
    Task<IEnumerable<InvoicePosEntity>> GetAllAsync(string? prefix, CancellationToken cancellationToken);
}
