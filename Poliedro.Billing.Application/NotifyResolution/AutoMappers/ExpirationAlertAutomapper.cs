using AutoMapper;
using Poliedro.Billing.Application.NotifyResolution.Dtos;
using Poliedro.Billing.Domain.NotifyResolution.Entities;
namespace Poliedro.Billing.Application.NotifyResolution.AutoMappers;
    public class ExpirationAlertAutomapper: Profile
    {
        public ExpirationAlertAutomapper()
        {
        CreateMap<ExpirationAlert, ExpirationAlertDto>()
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired))
            .ForMember(dest => dest.IsCloseToExpire, opt => opt.MapFrom(src => src.IsCloseToExpire))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ForMember(dest => dest.DaysToExpire, opt => opt.MapFrom(src => src.DaysToExpire))
            .ForMember(dest => dest.RemainingNumeration, opt => opt.MapFrom(src => src.RemainingNumeration))
            .ForMember(dest => dest.StatusNumeration, opt => opt.MapFrom(src => src.StatusNumeration));
    }
}