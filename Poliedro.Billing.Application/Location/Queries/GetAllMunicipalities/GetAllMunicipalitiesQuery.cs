using MediatR;
using Poliedro.Billing.Application.Common.Dtos;
using Poliedro.Billing.Application.Location.Dtos;

namespace Poliedro.Billing.Application.Location.Queries.GetAllMunicipalities;

public record GetAllMunicipalitiesQuery(string ApiKey)
    : IRequest<PagedResponseDto<MunicipalityDto>>;