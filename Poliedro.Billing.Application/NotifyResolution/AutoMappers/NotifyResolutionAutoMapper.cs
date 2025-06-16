using AutoMapper;
using Poliedro.Billing.Application.NotifyResolution.Dtos;
using Poliedro.Billing.Domain.Resolution.Entities;
namespace Poliedro.Billing.Application.NotifyResolution.AutoMappers;
    public class NotifyResolutionAutoMapper : Profile
    {
        public NotifyResolutionAutoMapper()
        {
            CreateMap<DianResolutionEntity, NotifyResolutionDto>()
               .ConstructUsing(src => new NotifyResolutionDto(
                   src.ExpirationDate,
                   src.ResolutionType.ToString(),
                   src.FinalRange,
                   src.CurrentlyNumber,
                   null
               ));
        }
    }