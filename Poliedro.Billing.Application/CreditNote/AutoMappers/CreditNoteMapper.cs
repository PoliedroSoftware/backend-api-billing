using AutoMapper;
using Poliedro.Billing.Application.CreditNote.Dtos;
using Poliedro.Billing.Domain.CreditNote.Entity;


namespace Poliedro.Billing.Application.CreditNote.AutoMappers;


public class CreditNoteMapper : Profile
{
    public CreditNoteMapper()
    {
        CreateMap<CreateCreditNoteInputDto, CreditNoteEntity>();
        CreateMap<CreditNoteEntity, CreditNoteDto>().ReverseMap();
    }
}
