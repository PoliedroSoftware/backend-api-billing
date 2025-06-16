using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Commands.CreateClient;

public class CreateClientCommandHandler
(
    IClientDomainService clientBillingElectronicDomainService,
    IMapper mapper
) : IRequestHandler<CreateClientCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var clientBillingElectronicEntity = mapper.Map<ClientEntity>(request);
        var result = await clientBillingElectronicDomainService.CreateAsync(clientBillingElectronicEntity, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
