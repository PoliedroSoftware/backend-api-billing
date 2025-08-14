using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;
namespace Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
public interface IBillingSenderFactory
{
    IBillingSender Resolve(string provider, ResolutionType typeResolution);
}
