using AutoMapper;
using Poliedro.Billing.Application.CustomersId.Dtos;
using Poliedro.Billing.Domain.CustomersId.Entities;
namespace Poliedro.Billing.Application.CustomersId.AutoMappers;
public class CustumersIdAutoMapper : Profile
{
    public CustumersIdAutoMapper()
    {
        CreateMap<Customers, CustomersDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.AdditionalEmails, opt => opt.MapFrom(src => src.AdditionalEmails))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.__V, opt => opt.MapFrom(src => src.__V))
            
            .ForMember(dest => dest.Dni, opt => opt.MapFrom(src => src.Dni))
            .ForMember(dest => dest.Tributary, opt => opt.MapFrom(src => src.Tributary));

        CreateMap<Dni, DniDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.VerificationDigit, opt => opt.MapFrom(src => src.VerificationDigit));

        CreateMap<Tributary, TributaryDto>()
            .ForMember(dest => dest.Liability, opt => opt.MapFrom(src => src.Liability))
            .ForMember(dest => dest.Regime, opt => opt.MapFrom(src => src.Regime))
            .ForMember(dest => dest.MerchantRegistration, opt => opt.MapFrom(src => src.MerchantRegistration))
            .ForMember(dest => dest.Liabilities, opt => opt.MapFrom(src => src.Liabilities));
    }
}

