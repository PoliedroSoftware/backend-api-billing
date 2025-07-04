using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Domain.Ports;
public interface ICreateBillingFactory
{
    Task<ICreateBillingStrategy> GetProcessorAsync(string resolutyonType, string provider);
}