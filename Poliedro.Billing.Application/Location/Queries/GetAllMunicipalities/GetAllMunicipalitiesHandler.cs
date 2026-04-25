using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Common.Dtos;
using Poliedro.Billing.Application.Location.Dtos;
using Poliedro.Billing.Domain.Location.Ports;

namespace Poliedro.Billing.Application.Location.Queries.GetAllMunicipalities;

public class GetAllMunicipalitiesHandler(
    IMunicipalityService municipalityService,
    IMapper mapper
    ) : IRequestHandler<GetAllMunicipalitiesQuery, PagedResponseDto<MunicipalityDto>>
{
    public async Task<PagedResponseDto<MunicipalityDto>> Handle(
        GetAllMunicipalitiesQuery request,
        CancellationToken cancellationToken)
    {
        var municipalities = await municipalityService.GetAllAsync(request.ApiKey, cancellationToken);
        var mapped = mapper.Map<IEnumerable<MunicipalityDto>>(municipalities).ToList();

        return new PagedResponseDto<MunicipalityDto>
        {
            Success = true,
            StatusCode = 200,
            Message = "Municipalities retrieved successfully",
            Data = mapped,
            Meta = new MetaDto
            {
                Total = mapped.Count,
                Page = 1,
                PageSize = mapped.Count,
                TotalPages = 1
            }
        };
    }
}