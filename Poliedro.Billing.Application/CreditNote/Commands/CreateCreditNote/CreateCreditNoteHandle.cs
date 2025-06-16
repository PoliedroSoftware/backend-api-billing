using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CreditNote.Entity;
using Poliedro.Billing.Domain.CreditNote.Ports;

namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;

public class CreateCreditNoteCommandHandler(ICreditNoteDomainService creditNoteService, IMapper mapper)
            : IRequestHandler<CreateCreditNoteCommand, Result<ApiResponseCreditNote, Error>>
{
    public async Task<Result<ApiResponseCreditNote, Error>> Handle(CreateCreditNoteCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<CreditNoteEntity>(request);
        return await creditNoteService.Create(entity, cancellationToken);
    }
}
