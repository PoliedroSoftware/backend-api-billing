namespace Poliedro.Billing.Domain.SuccessInvoice.Ports;
public interface ISuccessInvoiceRepository
{
    Task<IEnumerable<Entities.SuccessInvoice>> GetAllInvoice(SuccessInvoiceQueryParameters parameters);
}