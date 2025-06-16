using AutoMapper;
using Poliedro.Billing.Application.InvoicePendingWithDetails.Dtos;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Dtos;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Entities;

namespace Poliedro.Billing.Application.InvoicePendingWithDetails.AutoMappers;

public class InvoiceFEPendingWithDetailsProfile : Profile
{
    public InvoiceFEPendingWithDetailsProfile()
    {
        CreateMap<DetailsInvoiceFEPendingEntity, DetailsInvoiceFEPendingDto>();

        CreateMap<InvoiceFEPendingEntity, InvoiceFEPendingDto>()
            .ForMember(dest => dest.DetailsInvoicePendings, opt => opt.MapFrom(src => src.DetailsInvoicePendings));

        CreateMap<DetailsInvoiceFEPendingDto, DetailsInvoiceFEPendingEntity>();

        CreateMap<InvoiceFEPendingDto, InvoiceFEPendingEntity>()
            .ForMember(dest => dest.DetailsInvoicePendings, opt => opt.MapFrom(src => src.DetailsInvoicePendings));
    }

}