using MediatR;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.PrepareInvoicesBilling.Ports;

namespace Poliedro.Billing.Application.PrepareInvoicesBilling.Commands.PrepareInvoices;

public class PrepareInvoicesCommandHandler(
    IClientDomainService clientDomainService,
    IPrepareInvoicesBillingFactory prepareInvoicesBillingFactory
    ) : IRequestHandler<PrepareInvoicesCommand, List<object>>
{
    public async Task<List<object>> Handle(PrepareInvoicesCommand request, CancellationToken cancellationToken)
    {

        var client = await clientDomainService.GetByIdAsync(request.token, cancellationToken);

        if (client == null) throw new KeyNotFoundException("Cliente no encontrado.");

        var prepare = prepareInvoicesBillingFactory.GetProcessor(client.Value.DianResolution.ResolutionType.ToString());


        var processedInvoices = await prepare.PrepareInvoicesAsync(request.invoices, cancellationToken);

        return request.invoices;
        
    }
}