using AutoMapper;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;
using Poliedro.Billing.Application.CreditNote.Dtos;
using Poliedro.Billing.Domain.CreditNote.Entity;

namespace Poliedro.Billing.Application.CreditNote.AutoMappers;

public class CreditNoteMapper : Profile
{
    public CreditNoteMapper()
    {
        CreateMap<CreateCreditNoteCommand, CreditNoteEntity>()
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new Customer
            {
                ApiKey = src.ApiKey,
                IdentificationNumber = src.Customer.IdentificationNumber,
                Dv = src.Customer.Dv,
                Name = src.Customer.Name,
                Phone = src.Customer.Phone,
                Address = src.Customer.Address,
                Email = src.Customer.Email,
                MerchantRegistration = src.Customer.MerchantRegistration,
                TypeDocumentIdentificationId = src.Customer.TypeDocumentIdentificationId,
                TypeOrganizationId = src.Customer.TypeOrganizationId,
                TypeLiabilityId = src.Customer.TypeLiabilityId,
                MunicipalityId = src.Customer.MunicipalityId,
                TypeRegimeId = src.Customer.TypeRegimeId
            }));

        CreateMap<CreditNoteEntity, CreditNoteDto>().ReverseMap();
    }
}
