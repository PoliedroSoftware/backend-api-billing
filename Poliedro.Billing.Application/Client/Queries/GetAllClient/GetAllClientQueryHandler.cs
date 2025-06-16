using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.Client.Queries.GetAllClient;

public class GetAllClientQueryHandler
(
    IClientDomainService clientBillingElectronicDomainService,
    IMapper mapper
) : IRequestHandler<GetAllClientQuery, Result<IEnumerable<ClientDto>, Error>>
{

    public async Task<Result<IEnumerable<ClientDto>, Error>>
        Handle(GetAllClientQuery request, CancellationToken cancellationToken)
    {
        var result = await clientBillingElectronicDomainService.GetAllAsync(cancellationToken);
        if (!result.IsSuccess && result.Value != null)
            return result.Error!;

        return mapper.Map<List<ClientDto>>(result.Value);
    }
}