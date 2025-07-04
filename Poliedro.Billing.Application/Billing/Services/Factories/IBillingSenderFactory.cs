using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Factories;

public interface IBillingSenderFactory
{
    Task<IBillingSenderStrategy<TDto>> GetSenderAsync<TDto>(string Provider, string TypeResolution);
}
