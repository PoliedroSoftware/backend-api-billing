
using Poliedro.Billing.Domain.Resolution.Enums;
namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
public interface IBillingSenderFactory
{
    IBillingSender Resolve(int providerId, ResolutionType typeResolution);
}
