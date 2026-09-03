using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Poliedro.Billing.Domain.CreditNote.Entity;
using Poliedro.Billing.Domain.CreditNote.Ports;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CreditNote;

public class CreditNoteDomainService : ICreditNoteDomainService
{
    public Task<Result<ApiResponseCreditNote, Error>> Create(CreditNoteEntity creditNote, CancellationToken cancellationToken)
    {
        var error = Error.CreateInstance("NotImplemented", "CreditNoteDomainService stub: operation not implemented", HttpStatusCode.NotImplemented);
        return Task.FromResult(Result<ApiResponseCreditNote, Error>.Failure(error));
    }
}
