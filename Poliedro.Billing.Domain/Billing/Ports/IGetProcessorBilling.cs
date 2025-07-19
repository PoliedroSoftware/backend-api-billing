using Poliedro.Billing.Domain.Billing;

namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IGetProcessorBilling
{
    Task<ICreateBilling> GetProcessorAsync(string resolutyonType, string provider);
}