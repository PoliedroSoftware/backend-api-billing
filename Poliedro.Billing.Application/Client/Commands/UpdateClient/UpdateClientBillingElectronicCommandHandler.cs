using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Commands.UpdateClient
{
    public class UpdateClientBillingElectronicCommandHandler
    (
        IClientDomainService clientBillingElectronicDomainService,
        IMapper mapper
    ) : IRequestHandler<UpdateClientCommand, Result<VoidResult, Error>>
    {
        public async Task<Result<VoidResult, Error>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var clientBillingElectronicEntity = mapper.Map<ClientEntity>(request);

            var result = await clientBillingElectronicDomainService.UpdateAsync(clientBillingElectronicEntity, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }
}
