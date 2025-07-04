using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Enums;


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
        string Provider = providerType.ToString();

        ICreateBillingStrategy Processor = await _createBillingFactory.GetProcessorAsync(TypeResolution, Provider);
        IBillingSenderStrategy sender = await _billingSenderFactory.GetSenderAsync(Provider, TypeResolution);

        IEnumerable<Domain.Billing.CreateBilling> BillingEntities = mapper.Map<IEnumerable<Domain.Billing.CreateBilling>>(request.Invoices);

        IEnumerable<Domain.Billing.CreateBilling> ProcessedInvoices = await Processor.CreateInvoicesAsync(BillingEntities, CancellationToken);
        // create billig FOR provider and TypeResolution
        



        await sender.SendInvoicesAsync(ProcessedInvoices, CancellationToken);

        var billingDtos = mapper.Map<IEnumerable<CreateBillingDto>>(ProcessedInvoices);

        return billingDtos;

    }

}
