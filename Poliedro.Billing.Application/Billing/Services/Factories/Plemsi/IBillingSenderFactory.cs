using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;

public interface IBillingSenderFactory
{
    Task<IBillingSender<TDto>> GetSenderAsync<TDto>(string Provider, string TypeResolution);
}
