using MediatR;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Billing;

namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail;

public class CreateFERetailHandle(
    IClientDomainService _clientDomainService,
    IFERetailService _FERetailService) : IRequestHandler<CreateFERetailCommand, Result<BillingProviderResponse, Error>>
{
    public async Task<Result<BillingProviderResponse, Error>> Handle(CreateFERetailCommand request, CancellationToken cancellationToken)
    {
        var clients = await _clientDomainService.GetAllAsync(cancellationToken);
        var result = await _FERetailService.CreateElectronicInvoicesAsync(clients.Value!, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
