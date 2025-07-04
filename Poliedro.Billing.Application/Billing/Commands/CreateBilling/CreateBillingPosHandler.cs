using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Enums;
using Poliedro.Billing.Domain.Ports;


namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public class CreateBillingPosHandler(
    IClientDomainService _clientDomainService,
    ICreateBillingFactory _createBillingFactory,
    IBillingSenderFactory _billingSenderFactory,
    IMapper mapper
    ) : IRequestHandler<CreateBillingCommand, IEnumerable<CreateBillingDto>>
{
    public async Task<IEnumerable<CreateBillingDto>> Handle(CreateBillingCommand request, CancellationToken CancellationToken)
    {
        var client = await _clientDomainService.GetByIdAsync(request.ApiKey, CancellationToken);

        var TypeResolution = client.Value.DianResolution.ResolutionType.ToString();
        var providerType = (ProviderType)client.Value.ProviderId;
        var Provider = providerType.ToString();

        var Processor = await _createBillingFactory.GetProcessorAsync(TypeResolution, Provider);
        var sender = await _billingSenderFactory.GetSenderAsync(Provider, TypeResolution);

        IEnumerable<Poliedro.Billing.Domain.Billing.CreateBilling> BillingEntities = mapper.Map<IEnumerable<Poliedro.Billing.Domain.Billing.CreateBilling>>(request.Invoices);

        var ProcessedInvoices = await Processor.CreateInvoicesAsync(BillingEntities, CancellationToken);

        await sender.SendInvoicesAsync(ProcessedInvoices, CancellationToken);

        var billingDtos = mapper.Map<IEnumerable<CreateBillingDto>>(ProcessedInvoices);

        return billingDtos;

    }

}
