using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CreditNote.Entity;

namespace Poliedro.Billing.Domain.CreditNote.Ports;


public interface ICreditNoteDomainService
{
    Task<Result<ApiResponseCreditNote, Error>> Create(CreditNoteEntity creditNote, CancellationToken cancellationToken);
}
