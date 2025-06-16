using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.DianResolution.Dtos;

namespace Poliedro.Billing.Application.DianResolution.Queries.GetAllDianResolution;

public class GetAllDianResolutionQueryHandler(
    IDianResolutionDomainService dianResolutionDomainService,
    IMapper mapper
  ) : IRequestHandler<GetAllDianResolutionQuery, Result<IEnumerable<DianResolutionDto>, Error>>
{
    public async Task<Result<IEnumerable<DianResolutionDto>, Error>> Handle(GetAllDianResolutionQuery request, CancellationToken cancellationToken)
    {
        var result = await dianResolutionDomainService.GetAllAsync(cancellationToken);
        if (!result.IsSuccess && result.Value != null)
            return result.Error!;
        return mapper.Map<List<DianResolutionDto>>(result.Value);
    }
}
