using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
public interface ICreateBillingFactory
{
    Task<ICreateBilling> GetProcessorAsync(string resolutyonType, string provider);
}