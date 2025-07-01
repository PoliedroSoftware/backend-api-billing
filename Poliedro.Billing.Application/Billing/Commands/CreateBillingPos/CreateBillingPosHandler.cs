using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;


namespace Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;

public class CreateBillingPosHandler(
    IClientDomainService _clientDomainService,
    ICreateBillingFactory _createBillingFactory,
    IMapper mapper
    ) : IRequestHandler<CreateBillingCommand, List<CreateBilling>>
{
    public async Task<List<CreateBilling>> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientDomainService.GetByIdAsync(request.ApiKey, cancellationToken);
        if (client == null) throw new KeyNotFoundException("Cliente no encontrado.");
        var TypeResolution = client.Value.DianResolution.ResolutionType.ToString();
        var Provider = client.Value.ProviderId.ToString(); //enum
        if ( string.IsNullOrEmpty(Provider))
        {
            throw new ArgumentException($"Tipo de resolución o proveedor no especificado.{Provider}");
        }

        var processor = await _createBillingFactory.GetProcessorAsync(TypeResolution, Provider);
        List<CreateBilling> BillingEntities = mapper.Map<List<CreateBilling>>(request.Invoices);
        var processedInvoices = await processor.CreateInvoicesAsync(BillingEntities, cancellationToken);

        // call infrastructure to save the invoices 
        return request.Invoices;

    }

}
