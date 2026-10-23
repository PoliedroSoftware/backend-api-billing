using AutoMapper;
using Poliedro.Billing.Application.GetInvoice.Dtos;
using Poliedro.Billing.Domain.GetInvoice.Entities;

namespace Poliedro.Billing.Application.GetInvoice.AutoMappers;

public class GetInvoiceMapper : Profile
{
    public GetInvoiceMapper()
    {
        CreateMap<GetInvoiceEntity, GetInvoiceDto>()
            .ForMember(destination => destination.InvoiceLine, options => options.MapFrom(source => source.InvoiceLines))
            .ForMember(destination => destination.Attachment, options => options.MapFrom(source => source.Attachments))
            .ReverseMap()
            .ForMember(destination => destination.InvoiceLines, options => options.MapFrom(source => source.InvoiceLine))
            .ForMember(destination => destination.Attachments, options => options.MapFrom(source => source.Attachment));
    }
}
