using MediatR;
using Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;

namespace Poliedro.Billing.Application.BillingCreditNote.Commands.CreditNote;
public record CreateCreditNoteCommand(IEnumerable<CreateCreditNoteInputDTO> Invoices, string ApiKey) : IRequest<IEnumerable<CreditNoteDTO>>;