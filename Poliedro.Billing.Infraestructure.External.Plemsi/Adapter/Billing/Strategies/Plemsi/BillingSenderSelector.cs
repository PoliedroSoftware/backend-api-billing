using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Strategies.Plemsi;

public class BillingSenderSelector(IServiceProvider serviceProvider) : IBillingSenderFactory
{
    public Task<IBillingSender<TDto>> GetSenderAsync<TDto>(string Provider, string TypeResolution)
    {
        object sender = (Provider, TypeResolution) switch
        {
            ("PLEMSI", "FE") when typeof(TDto) == typeof(PlemiFEInvoiceDTO)
                => serviceProvider.GetRequiredService<BillingSenderFE>(),
            ("PLEMSI", "POS") when typeof(TDto) == typeof(PlemiPOSInvoiceDTO)
                => serviceProvider.GetRequiredService<BillingSenderPOS>(),
            _ => throw new ArgumentException($"Unknown sender for provider {Provider} and resolutionType {TypeResolution}")
        };

        return Task.FromResult((IBillingSender<TDto>)sender);

    }
}
