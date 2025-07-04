
namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingSenderFactory
{
    Task<IBillingSenderStrategy> GetSenderAsync(string Provider, string TypeResolution);
}
