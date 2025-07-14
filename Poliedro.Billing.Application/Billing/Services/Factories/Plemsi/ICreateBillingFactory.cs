using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
public interface ICreateBillingFactory<TInput, TOutput>
{
    Task<ICreateBilling<CreateBilling, TOutput>> GetProcessorAsync(string resolutyonType, string provider);
}