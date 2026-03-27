using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolutionCreditNote.DomineService.Impl;


public class DianResolutionCreditNoteDomainService(DataBaseContext _context) : IDianResolutionCreditNote
{
    public async Task<Result<DianResolutionCreditNoteEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var dianResolutionCreditNote = await _context.DianResolutionCreditNote
            .FindAsync(id, cancellationToken);

        if (dianResolutionCreditNote is null)
            return null!;

        return dianResolutionCreditNote;
    }
}
