
using AutoMapper;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Domain.CompanyProvider.Entities;

namespace Poliedro.Billing.Application.CompanyProvider.AutoMappers;
public class CompanyProviderMapper: Profile
{
    public CompanyProviderMapper()
    {
        CreateMap<CompanyProviderEntity, CompanyProviderDto>()
     .ConstructUsing(src => new CompanyProviderDto(
         src.CompanyProviderId,
         src.CompanyId,
         src.ProviderId,
         src.ServiceId,
         src.ApiUser,
         src.ApiPassword,
         src.ApiKey,
         src.EnvironmentType,
         src.HeadNote,
         src.FooterNote,
         src.Active
     ))
     .ReverseMap();

        CreateMap<CreateCompanyProviderDto, CompanyProviderEntity>();   // ← NUEVO
        CreateMap<UpdateCompanyProviderDto, CompanyProviderEntity>();   // ← NUEVO
    }   
}
