using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.DianResolution.Dtos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;

namespace Poliedro.Billing.Application.DianResolution.Queries.GetDianResolutionById
{
    public class GetDianResolutionByIdQueryHandler(
        IDianResolutionDomainService dianResolutionDomainService,
        IMapper mapper)
        : IRequestHandler<GetDianResolutionByIdQuery, Result<DianResolutionDto, Error>>
    {
        public async Task<Result<DianResolutionDto, Error>> Handle(GetDianResolutionByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await dianResolutionDomainService.GetByIdAsync(request.Id, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!;

            return mapper.Map<DianResolutionDto>(result.Value);
        }
    }
}
