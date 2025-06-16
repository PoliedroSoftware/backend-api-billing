using Poliedro.Billing.Domain.GetInvoice.Entities;

namespace Poliedro.Billing.Domain.GetInvoice.Ports
{
    public interface IGetInvoiceRepository
    {
        Task<InvoiceData?> GetByIdAsync(string id);

    }
}
