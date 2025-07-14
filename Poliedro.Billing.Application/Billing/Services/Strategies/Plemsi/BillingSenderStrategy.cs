
using AutoMapper;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Strategies.Plemsi;

public class BillingSenderStrategy
    (
    IBillingSenderFactory senderFactory,
    IMapper mapper
    ): IBillingSenderOrchestrator
{
    public async Task SendInvoicesAsync(
    IEnumerable<Domain.Billing.CreateBilling> invoices,
    string provider,
    string typeResolution,
    CancellationToken cancellationToken)
    {
        switch (provider, typeResolution)
        {
            case ("PLEMSI", "FE"):
                {
                    var dtoList = mapper.Map<IEnumerable<PlemiFEInvoiceDTO>>(invoices);
                    var sender = await senderFactory.GetSenderAsync<PlemiFEInvoiceDTO>(provider, typeResolution);
                    await sender.SendInvoicesAsync(dtoList, cancellationToken);
                    break;
                }
            case ("PLEMSI", "POS"):
                {
                    var dtoList = mapper.Map<IEnumerable<PlemiPOSInvoiceDTO>>(invoices);
                    var sender = await senderFactory.GetSenderAsync<PlemiPOSInvoiceDTO>(provider, typeResolution);
                    await sender.SendInvoicesAsync(dtoList, cancellationToken);
                    break;
                }
            default:
                throw new ArgumentException($"Unsupported provider/typeResolution combination: ({provider}, {typeResolution})");
        }
    }

}
