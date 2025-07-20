using Poliedro.Billing.Domain.Billing.Ports;
namespace Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
public interface IBillingSenderFactory
{
    IBillingSender Resolve(string provider, string typeResolution);
}
