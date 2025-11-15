using AutoMapper;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
using Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.FERetail.Entity;
using TaxTotalEntity = Poliedro.Billing.Domain.FERetail.Entity.TaxTotalEntity;

namespace Poliedro.Billing.Application.BillingCreditNote.AutoMappers;

public class CreditNoteAutoMapper : Profile
{
    public CreditNoteAutoMapper()
    {

        CreateMap<CreateBilling, CreditNoteDTO>();//output
        CreateMap<CreateCreditNoteInputDTO, CreateBilling>();//intput


        //CreateMap<SoftwareManufacturerEntity, SoftwareManufacturerDTO>().ReverseMap();
        CreateMap<PayPointInfoEntity, PayPointInfoDTO>().ReverseMap();
        //CreateMap<OrderReferenceEntity, OrderReferenceDTO>().ReverseMap();
        //CreateMap<AttachmentEntity, AttachmentDTO>().ReverseMap();

        CreateMap<InvoiceReferenceEntity, InvoiceReferenceDTO>().ReverseMap();
        CreateMap<DiscrepancyEntity, DiscrepancyDTO>().ReverseMap();

        CreateMap<CustomerEntity, CustomerBillingDTO>().ReverseMap();
        CreateMap<PaymentEntity, PaymentDTO>().ReverseMap();
        CreateMap<GeneralAllowanceEntity, GeneralAllowanceDTO>().ReverseMap();
        CreateMap<ItemElectronicEntity, ItemElectronicDTO>().ReverseMap();
        CreateMap<AllowanceChargeEntity, AllowanceChargeDTO>().ReverseMap();
        CreateMap<TaxTotalEntity, TaxTotalDTO>().ReverseMap();
        CreateMap<WIthHoldingTaxTotalEntity, WIthHoldingTaxTotalDTO>().ReverseMap();
        CreateMap<AllTaxTotalEntity, AllTaxTotalDTO>().ReverseMap();
        CreateMap<AllHoldingsTaxTotalEntity, AllHoldingsTaxTotalDTO>().ReverseMap();

        CreateMap<FERetailelectronicEntity, CreditNoteDTO>()
           //.ForMember(dest => dest.date, opt => opt.MapFrom(src => src.date))
           //.ForMember(dest => dest.time, opt => opt.MapFrom(src => src.time))
           .ForMember(dest => dest.prefix, opt => opt.MapFrom(src => src.prefix))
           .ForMember(dest => dest.number, opt => opt.MapFrom(src => src.number))
           .ForMember(dest => dest.send_email, opt => opt.MapFrom(src => src.send_email))
           .ForMember(dest => dest.resolution, opt => opt.MapFrom(src => src.resolution))
           .ForMember(dest => dest.resolutionText, opt => opt.MapFrom(src => src.resolutionText))
           .ForMember(dest => dest.head_note, opt => opt.MapFrom(src => src.head_note))
           .ForMember(dest => dest.foot_note, opt => opt.MapFrom(src => src.foot_note))
           .ForMember(dest => dest.notes, opt => opt.MapFrom(src => src.notes))
           .ForMember(dest => dest.allowanceTotal, opt => opt.MapFrom(src => src.allowanceTotal))
           .ForMember(dest => dest.invoiceBaseTotal, opt => opt.MapFrom(src => src.invoiceBaseTotal))
           .ForMember(dest => dest.invoiceTaxExclusiveTotal, opt => opt.MapFrom(src => src.invoiceTaxExclusiveTotal))
           .ForMember(dest => dest.invoiceTaxInclusiveTotal, opt => opt.MapFrom(src => src.invoiceTaxInclusiveTotal))
           .ForMember(dest => dest.totalToPay, opt => opt.MapFrom(src => src.totalToPay));
        //.ForMember(dest => dest.finalTotalToPay, opt => opt.MapFrom(src => src.finalTotalToPay));
        //CreateMap<OrderReferenceEntity, OrderReferenceRequestFEDTO>()
        //    .ForMember(dest => dest.id_order, opt => opt.MapFrom(src => src.IdOrder));
        //CreateMap<AttachmentEntity, AttachmentRequestFEDTO>()
        //    .ForMember(dest => dest.filename, opt => opt.MapFrom(src => src.FileName))
        //    .ForMember(dest => dest.b64data, opt => opt.MapFrom(src => src.B64Data));

        CreateMap<InvoiceReferenceEntity, InvoiceReferenceDTO>()
            .ForMember(dest => dest.number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.uuid, opt => opt.MapFrom(src => src.UuId))
            .ForMember(dest => dest.issue_date, opt => opt.MapFrom(src => src.IssueDate));

        CreateMap<DiscrepancyEntity, DiscrepancyDTO>()
            .ForMember(dest => dest.code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description));

        CreateMap<CustomerEntity, CustomerRequestFEDTO>()
            .ForMember(dest => dest.identification_number, opt => opt.MapFrom(src => src.IdentificationNumber))
            .ForMember(dest => dest.dv, opt => opt.MapFrom(src => src.Dv))
            .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.merchant_registration, opt => opt.MapFrom(src => src.MerchantRegistration))
            .ForMember(dest => dest.type_document_identification_id, opt => opt.MapFrom(src => src.TypeDocumentIdentificationId))
            .ForMember(dest => dest.type_organization_id, opt => opt.MapFrom(src => src.TypeOrganizationId))
            .ForMember(dest => dest.type_liability_id, opt => opt.MapFrom(src => src.TypeLiabilityId))
            .ForMember(dest => dest.municipality_id, opt => opt.MapFrom(src => src.MunicipalityId))
            .ForMember(dest => dest.municipality_code, opt => opt.MapFrom(src => src.MunicipalityCode))
            .ForMember(dest => dest.type_regime_id, opt => opt.MapFrom(src => src.TypeRegimeId));
        CreateMap<PaymentEntity, PaymentRequestFEDTO>()
            .ForMember(dest => dest.payment_form_id, opt => opt.MapFrom(src => src.PaymentFormId))
            .ForMember(dest => dest.payment_method_id, opt => opt.MapFrom(src => src.PaymentMethodId))
            .ForMember(dest => dest.payment_due_date, opt => opt.MapFrom(src => src.PaymentDueDate))
            .ForMember(dest => dest.duration_measure, opt => opt.MapFrom(src => src.DurationMeasure));
        CreateMap<GeneralAllowanceEntity, GeneralAllowanceRequestFEDTO>()
            .ForMember(dest => dest.allowance_charge_reason, opt => opt.MapFrom(src => src.AllowanceChargeReason))
            .ForMember(dest => dest.allowance_percent, opt => opt.MapFrom(src => src.AllowancePercent))
            .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.base_amount, opt => opt.MapFrom(src => src.BaseAmount));
        CreateMap<ItemElectronicEntity, ItemElectronicRequestFEDTO>()
            .ForMember(dest => dest.unit_measure_id, opt => opt.MapFrom(src => src.UnitMeasureId))
            .ForMember(dest => dest.line_extension_amount, opt => opt.MapFrom(src => src.LineExtensionAmount))
            .ForMember(dest => dest.free_of_charge_indicator, opt => opt.MapFrom(src => src.FreeOfChargeIndicator))
            .ForMember(dest => dest.allowance_charges, opt => opt.MapFrom(src => src.AllowanceCharges))
            .ForMember(dest => dest.tax_totals, opt => opt.MapFrom(src => src.TaxTotals))
            .ForMember(dest => dest.with_holding_tax_total, opt => opt.MapFrom(src => src.WithHoldingTaxTotal))

            .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.type_item_identification_id, opt => opt.MapFrom(src => src.TypeItemIdentificationId))
            .ForMember(dest => dest.price_amount, opt => opt.MapFrom(src => src.PriceAmount))
            .ForMember(dest => dest.base_quantity, opt => opt.MapFrom(src => src.BaseQuantity))
            .ForMember(dest => dest.invoiced_quantity, opt => opt.MapFrom(src => src.InvoicedQuantity));
        CreateMap<AllowanceChargeEntity, AllowanceChargeRequestFEDTO>()
            .ForMember(dest => dest.charge_indicator, opt => opt.MapFrom(src => src.ChargeIndicator))
            .ForMember(dest => dest.allowance_charge_reason, opt => opt.MapFrom(src => src.AllowanceChargeReason))
            .ForMember(dest => dest.multiplier_factor_numeric, opt => opt.MapFrom(src => src.MultiplierFactorNumeric))
            .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.base_amount, opt => opt.MapFrom(src => src.BaseAmount));
        CreateMap<TaxTotalEntity, TaxTotalRequestFEDTO>()
            .ForMember(dest => dest.tax_id, opt => opt.MapFrom(src => src.TaxId))
            .ForMember(dest => dest.percent, opt => opt.MapFrom(src => src.Percent))
            .ForMember(dest => dest.tax_amount, opt => opt.MapFrom(src => src.TaxAmount))
            .ForMember(dest => dest.taxable_amount, opt => opt.MapFrom(src => src.TaxableAmount));
        CreateMap<WIthHoldingTaxTotalEntity, WIthHoldingTaxTotalRequestFEDTO>()
            .ForMember(dest => dest.tax_id, opt => opt.MapFrom(src => src.TaxId))
            .ForMember(dest => dest.percent, opt => opt.MapFrom(src => src.Percent))
            .ForMember(dest => dest.tax_amount, opt => opt.MapFrom(src => src.TaxAmount))
            .ForMember(dest => dest.taxable_amount, opt => opt.MapFrom(src => src.TaxableAmount));
        CreateMap<AllTaxTotalEntity, AllTaxTotalRequestFEDTO>()
            .ForMember(dest => dest.tax_id, opt => opt.MapFrom(src => src.TaxId))
            .ForMember(dest => dest.percent, opt => opt.MapFrom(src => src.Percent))
            .ForMember(dest => dest.tax_amount, opt => opt.MapFrom(src => src.TaxAmount))
            .ForMember(dest => dest.taxable_amount, opt => opt.MapFrom(src => src.TaxableAmount));
        CreateMap<AllHoldingsTaxTotalEntity, AllHoldingsTaxTotalRequestFEDTO>()
            .ForMember(dest => dest.tax_id, opt => opt.MapFrom(src => src.TaxId))
            .ForMember(dest => dest.percent, opt => opt.MapFrom(src => src.Percent))
            .ForMember(dest => dest.tax_amount, opt => opt.MapFrom(src => src.TaxAmount))
            .ForMember(dest => dest.taxable_amount, opt => opt.MapFrom(src => src.TaxableAmount));
            //CreateMap<CustomSubtotalEntity, CustomSubtotalRequestFEDTO>()
            //    .ForMember(dest => dest.concept, opt => opt.MapFrom(src => src.Concept))
            //    .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.Amount));
    }
}
