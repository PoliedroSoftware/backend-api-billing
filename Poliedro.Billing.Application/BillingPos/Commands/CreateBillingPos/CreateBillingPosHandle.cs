using MediatR;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.DomainService;


namespace Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;

public class CreateBillingPosHandle(
    IClientDomainService _clientDomainService,
    ICreateBillingFactory _createBillingFactory
    ) : IRequestHandler<CreateBillingCommand, List<CreateBilling>>
{
    public async Task<List<CreateBilling>> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientDomainService.GetByIdAsync(request.token, cancellationToken);

        if (client == null) throw new KeyNotFoundException("Cliente no encontrado.");

        var TypeResolution = client.Value.DianResolution.ResolutionType.ToString();



        var Provider = client.Value.ProviderId.ToString(); //enum

        if( string.IsNullOrEmpty(Provider))
        {
            throw new ArgumentException($"Tipo de resolución o proveedor no especificado.{Provider}");
        }

        var processor = await _createBillingFactory.GetProcessorAsync(TypeResolution, Provider);

        var processedInvoices = await processor.CreateInvoicesAsync(request.invoices, cancellationToken);

        return request.invoices;

    }

}
