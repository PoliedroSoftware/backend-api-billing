using Poliedro.Billing.Domain.InvoicePos.Entities;

namespace Poliedro.Billing.Domain.InvoicePos.DomainService;

public interface IInvoicePosDomainService
{
    Task<IEnumerable<InvoicePosEntity>> GetAllAsync(string? prefix, CancellationToken cancellationToken);
}
