namespace Poliedro.Billing.Domain.BillingPos.Ports;
public interface ICreateBillingFactory
{
    Task<ICreateBillingStrategy> GetProcessorAsync(string resolutyonType, string provider);
}