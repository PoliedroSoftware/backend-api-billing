using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Factories;
public interface ICreateBillingFactory
{
    Task<ICreateBillingStrategy> GetProcessorAsync(string resolutyonType, string provider);
}