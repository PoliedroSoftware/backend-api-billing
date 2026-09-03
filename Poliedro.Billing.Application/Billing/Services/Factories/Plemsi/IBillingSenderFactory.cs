using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Enums;
namespace Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
public interface IBillingSenderFactory
{
    IBillingSender Resolve(ProviderType provider, ResolutionType typeResolution);
}
