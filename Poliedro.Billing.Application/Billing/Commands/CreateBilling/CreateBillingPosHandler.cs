using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Enums;

namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public class CreateBillingPosHandler(
    IClientDomainService _clientDomainService,
    ICreateBillingFactory _createBillingFactory,
    IBillingSenderOrchestrator _billingSenderOrchestrator,   
    IMapper mapper
    ) : IRequestHandler<CreateBillingCommand, IEnumerable<CreateBillingDTO>>
{
    public async Task<IEnumerable<CreateBillingDTO>> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientDomainService.GetByIdAsync(request.ApiKey, cancellationToken);

        string typeResolution = client.Value.DianResolution.ResolutionType.ToString();
        var providerType = (ProviderType)client.Value.ProviderId;
        string provider = providerType.ToString();

        var processor = await _createBillingFactory.GetProcessorAsync(typeResolution, provider);

        // DTOs de entrada a CreateBilling
        var billingEntities = mapper.Map<IEnumerable<Domain.Billing.CreateBilling>>(request.Invoices);

        // Pasa como object
        var processedInvoices = await processor.CreateInvoicesAsync(billingEntities, cancellationToken);

        // Enviar las facturas procesadas
          await _billingSenderOrchestrator.SendInvoicesAsync(processedInvoices , provider, typeResolution, cancellationToken);

        // Después haces cast o map a tus DTOs finales
        var billingDtos = mapper.Map<IEnumerable<CreateBillingDTO>>(processedInvoices);



        return billingDtos;
    }
}
