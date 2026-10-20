
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Enums;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IGetProcessorBilling
{
    Task<ICreateBilling> GetProcessorAsync(ResolutionType resolutyonType, ProviderType provider);
}