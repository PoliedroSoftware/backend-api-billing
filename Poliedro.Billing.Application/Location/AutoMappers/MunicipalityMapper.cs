using AutoMapper;
using Poliedro.Billing.Application.Location.Dtos;
using Poliedro.Billing.Domain.Location.Entities;

namespace Poliedro.Billing.Application.Location.AutoMappers;

public class MunicipalityMapper : Profile
{
    public MunicipalityMapper()
    {
        CreateMap<MunicipalityEntity, MunicipalityDto>();
    }
}