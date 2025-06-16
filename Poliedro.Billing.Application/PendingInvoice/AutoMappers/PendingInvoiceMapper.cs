using AutoMapper;
using Poliedro.Billing.Application.PendingInvoice.Dtos;
using Poliedro.Billing.Domain.PedingInvoice.Entities;

namespace Poliedro.Billing.Application.PendingInvoice.AutoMappers;

public class PendingInvoiceMapper : Profile
{
    public PendingInvoiceMapper()
    {
       
        CreateMap<PedingInvoiceEntity, PedingInvoiceDto>().ReverseMap();
    }
}
