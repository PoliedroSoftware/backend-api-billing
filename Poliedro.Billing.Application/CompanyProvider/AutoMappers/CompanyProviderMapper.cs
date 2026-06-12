
using AutoMapper;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Domain.CompanyProvider.Entities;

namespace Poliedro.Billing.Application.CompanyProvider.AutoMappers;
public class CompanyProviderMapper: Profile
{
    public CompanyProviderMapper()
    {
        CreateMap<CompanyProviderEntity, CompanyProviderDto>() .ReverseMap();
    }   
}
