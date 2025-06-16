using AutoMapper;
using Poliedro.Billing.Domain.GetInvoice.Entities;

namespace Poliedro.Billing.Application.GetInvoice.AutoMappers;

public class GetInvoiceMapper : Profile
{
    public GetInvoiceMapper()
    {
        CreateMap<GetInvoiceEntity, GetInvoiceDto>().ReverseMap();
    }
}
