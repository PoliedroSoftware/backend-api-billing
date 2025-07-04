using AutoMapper;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Application.Billing.AutoMappers;

public class BillingAutoMapper : Profile
{
    public BillingAutoMapper()
    {
        CreateMap<CreateBilling, CreateBillingDto>();

        CreateMap<SoftwareManufacturerEntity, SoftwareManufacturerDTO>().ReverseMap();
        CreateMap<PayPointInfoEntity, PayPointInfoDTO>().ReverseMap();
        CreateMap<OrderReferenceEntity, OrderReferenceDTO>().ReverseMap();
        CreateMap<AttachmentEntity, AttachmentDTO>().ReverseMap();
        CreateMap<CustomerEntity, CustomerBillingDTO>().ReverseMap();
        CreateMap<PaymentEntity, PaymentDTO>().ReverseMap();
        CreateMap<GeneralAllowanceEntity, GeneralAllowanceDTO>().ReverseMap();
        CreateMap<ItemElectronicEntity, ItemElectronicDTO>().ReverseMap();
        CreateMap<AllowanceChargeEntity, AllowanceChargeDTO>().ReverseMap();
        CreateMap<TaxTotalEntity, TaxTotalDTO>().ReverseMap();
        CreateMap<WIthHoldingTaxTotalEntity, WIthHoldingTaxTotalDTO>().ReverseMap();
        CreateMap<AllTaxTotalEntity, AllTaxTotalDTO>().ReverseMap();
        CreateMap<AllHoldingsTaxTotalEntity, AllHoldingsTaxTotalDTO>().ReverseMap();
    }
}