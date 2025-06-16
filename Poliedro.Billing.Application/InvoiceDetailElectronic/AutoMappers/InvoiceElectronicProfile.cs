using AutoMapper;
using Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Entities;
namespace Poliedro.Billing.Application.InvoiceDetailElectronic.AutoMappers;

public class InvoiceElectronicProfile : Profile
{
    public InvoiceElectronicProfile()
    {
        CreateMap<InvoiceWithDetailElectronic, InvoiceWithDetailElectronicDto>();

        CreateMap<InvoiceElectronic, InvoiceElectronicDto>()
            .ForMember(dest => dest.InvoiceElectronicDetails, opt => opt.MapFrom(src => src.InvoiceElectronicDetails)); 

        CreateMap<InvoiceWithDetailElectronicDto, InvoiceWithDetailElectronic>();

        CreateMap<InvoiceElectronicDto, InvoiceElectronic>()
            .ForMember(dest => dest.InvoiceElectronicDetails, opt => opt.MapFrom(src => src.InvoiceElectronicDetails)); 

    }
}