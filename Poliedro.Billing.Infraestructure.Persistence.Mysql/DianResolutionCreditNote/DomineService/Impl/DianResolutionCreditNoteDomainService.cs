using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolutionCreditNote.DomineService.Impl;

public class DianResolutionCreditNoteDomainService
    (
    DataBaseContext _context
    ) : IDianResolutionCreditNote
{
    public async Task<Result<DianResolutionEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var DianResolutionCreditNote = await _context.DianResolutionCreditNote
            .FindAsync(id, cancellationToken);
        if (DianResolutionCreditNote != null)
        {
            return DianResolutionCreditNote;
        }
        return null;
    }
}
