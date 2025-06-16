using AutoMapper;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;
using Poliedro.Billing.Application.CreditNote.Dtos;
using Poliedro.Billing.Domain.CreditNote.Entity;

namespace Poliedro.Billing.Application.CreditNote.AutoMappers
{
    public class CreditNoteMapper : Profile
    {
        public CreditNoteMapper()
        {
            CreateMap<CreditNoteEntity, CreditNoteDto>().ReverseMap();
            CreateMap<CreditNoteEntity, CreateCreditNoteCommand>().ReverseMap();
           
        }
    }
}
