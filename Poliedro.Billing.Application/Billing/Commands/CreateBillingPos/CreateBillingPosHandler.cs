using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Enums;


namespace Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;

public class CreateBillingPosHandler(
    IClientDomainService _clientDomainService,
    ICreateBillingFactory _createBillingFactory,
    IBillingSender _billingSender,
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

        IEnumerable<CreateBilling> BillingEntities = mapper.Map<IEnumerable<CreateBilling>>(request.Invoices);

        var ProcessedInvoices = await Processor.CreateInvoicesAsync(BillingEntities, CancellationToken);

        await _billingSender.SendInvoicesAsync(ProcessedInvoices, CancellationToken);

        var billingDtos = mapper.Map<IEnumerable<CreateBillingDto>>(ProcessedInvoices);

        return billingDtos;

    }

}
