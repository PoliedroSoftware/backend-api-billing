using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Queries.GetClientBillingElectronicById
{
    public class GetClientBillingElectronicByIdQueryHandler(
        IClientDomainService clientBillingElectronicDomainService,
        IMapper mapper)
        : IRequestHandler<GetClientByIdQuery, Result<ClientDto, Error>>
    {
        public async Task<Result<ClientDto, Error>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await clientBillingElectronicDomainService.GetByIdAsync(request.Id, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!;

            return mapper.Map<ClientDto>(result.Value);
        }
    }
}
