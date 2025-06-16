using Poliedro.Billing.Domain.GetInvoice.Entities;

namespace Poliedro.Billing.Domain.GetInvoice.DomainGetInvoice;

public interface IGetInvoiceDomainGetInvoice
{
    Task<GetInvoiceEntity?> GetByIdAsync(
    string cufe, string token,
    CancellationToken cancellationToken);
}


