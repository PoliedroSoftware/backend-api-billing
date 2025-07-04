namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBillingFactory
{
    Task<ICreateBillingStrategy> GetProcessorAsync(string resolutyonType, string provider);
}