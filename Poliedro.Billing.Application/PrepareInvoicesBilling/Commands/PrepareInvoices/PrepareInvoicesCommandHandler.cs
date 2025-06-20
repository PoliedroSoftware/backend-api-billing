using MediatR;

namespace Poliedro.Billing.Application.PrepareInvoicesBilling.Commands.PrepareInvoices;

public class PrepareInvoicesCommandHandler : IRequestHandler<PrepareInvoicesCommand, List<object>>
{
    public async Task<List<object>> Handle(PrepareInvoicesCommand request, CancellationToken cancellationToken)
    {
        return request.invoices;
    }
}