using MediatR;
using Poliedro.Billing.Application.CreditNote.Dtos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CreditNote.Entity;


namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;


public record CreateCreditNoteCommand(
    int Id,
    IEnumerable<CreateCreditNoteInputDto> CreditNotes
) : IRequest<IEnumerable<CreateCreditNoteResultDto>>;
