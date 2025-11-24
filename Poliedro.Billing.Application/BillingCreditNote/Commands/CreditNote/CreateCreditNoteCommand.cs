using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;

namespace Poliedro.Billing.Application.BillingCreditNote.Commands.CreditNote;
public record CreateCreditNoteCommand(IEnumerable<CreateBillingInputDTO> Invoices, string ApiKey) : IRequest<IEnumerable<CreateBillingResultDTO>>;