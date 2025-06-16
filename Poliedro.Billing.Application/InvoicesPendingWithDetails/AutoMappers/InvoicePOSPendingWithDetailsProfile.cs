using AutoMapper;
using Poliedro.Billing.Application.InvoicePendingWithDetails.Dtos;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Dtos;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Entities;


namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.AutoMappers;

public class InvoicePOSPendingWithDetailsProfile : Profile
{
    public InvoicePOSPendingWithDetailsProfile()
    {
        CreateMap<DetailsInvoicePOSPendingEntity, DetailsInvoicePOSPendingDto>();

        CreateMap<InvoicePOSPendingEntity, InvoicePOSPendingDto>()
            .ForMember(dest => dest.DetailsInvoicePendings, opt => opt.MapFrom(src => src.DetailsInvoicePendings));

        CreateMap<DetailsInvoiceFEPendingDto, DetailsInvoiceFEPendingEntity>();

        CreateMap<InvoicePOSPendingDto, InvoicePOSPendingEntity>()
            .ForMember(dest => dest.DetailsInvoicePendings, opt => opt.MapFrom(src => src.DetailsInvoicePendings));
    }
}
