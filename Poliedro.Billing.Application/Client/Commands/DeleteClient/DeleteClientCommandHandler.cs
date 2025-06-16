using MediatR;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
namespace Poliedro.Billing.Application.Client.Commands.DeleteClient;

public class DeleteClientCommandHandler
(
    IClientDomainService clientBillingElectronicDomainService
) : IRequestHandler<DeleteClientCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var result = await clientBillingElectronicDomainService.DeleteAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        return result.Value!;
    }
}
