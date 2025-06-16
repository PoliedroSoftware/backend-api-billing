using System;
using AutoMapper;
using System.Collections.Generic;
using Poliedro.Billing.Application.SuccessInvoice.Dtos;
using Poliedro.Billing.Domain.SuccessInvoice.Entities;
namespace Poliedro.Billing.Application.SuccessInvoice.AutoMappers;
    public  class SuccessInvoiceAutoMapper : Profile
    {
        public SuccessInvoiceAutoMapper() {

        CreateMap<Poliedro.Billing.Domain.SuccessInvoice.Entities.SuccessInvoice, SuccessInvoiceDto>()
           .ForMember(dest => dest._Id, opt => opt.MapFrom(src => src._Id))
            .ForMember(dest => dest.Resolution_Number, opt => opt.MapFrom(src => src.Resolution_Number))
            .ForMember(dest => dest.Type_Document_Id, opt => opt.MapFrom(src => src.Type_Document_Id))
            .ForMember(dest => dest.Order_Reference, opt => opt.MapFrom(src => src.Order_Reference))
            .ForMember(dest => dest.Payment_Form, opt => opt.MapFrom(src => src.Payment_Form))
            .ForMember(dest => dest.Legal_Monetary_Totals, opt => opt.MapFrom(src => src.Legal_Monetary_Totals));
        CreateMap<Customer, CustomerDto>();
        CreateMap<OrderReference, OrderReferenceDto>();
        CreateMap<PaymentForm, PaymentFormDto>();
        CreateMap<LegalMonetaryTotals, LegalMonetaryTotalsDto>();
    }
    }
