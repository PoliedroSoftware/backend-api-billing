using MediatR;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Server.DomainService;

namespace Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;

public class CreateBillingPosHandle(
    IClientDomainService _clientDomainService,
    IBillingService _billingService,
    IInvoicePos invoiceRepository,
    IServerDomainService _serverDomainService) : IRequestHandler<CreateBillingPosCommand, Result<ApiResponseBillingPos, Error>>
{
    private readonly IInvoicePos invoiceRepository = invoiceRepository;
    private readonly IServerDomainService serverDomainService = _serverDomainService;

    public async Task<Result<ApiResponseBillingPos, Error>> Handle(CreateBillingPosCommand request, CancellationToken cancellationToken)
    {
        var clients = await _clientDomainService.GetAllAsync(cancellationToken);
        var result = await _billingService.CreateInvoicesPosAsync(clients.Value!, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
