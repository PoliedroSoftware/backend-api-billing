using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Domain.Resolution.DomainService;

public interface IDianResolutionCreditNote
{
    Task<Result<DianResolutionCreditNoteEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken); 
}
