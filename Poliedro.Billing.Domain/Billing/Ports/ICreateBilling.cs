namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBilling
{
    Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(
        IEnumerable<CreateBilling> invoices, DateTime ExpirationDate, int FinalRange, string Prefix, string Apikey, CancellationToken cancellationToken);
}