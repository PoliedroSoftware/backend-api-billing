using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.Siigo.Ports;
using Poliedro.Billing.Application.Siigo.Contracts.Responses;

namespace Poliedro.Billing.Application.Siigo.Commands
{
    internal class CreateInvoicesSiigoHandler(
        ISiigoGateway _clientDomainService) : IRequestHandler<CreateInvoicesSiigoCommand, Result<ApiResponseSiigo, Error>>
    {

        public async Task<Result<ApiResponseSiigo, Error>> Handle(CreateInvoicesSiigoCommand request, CancellationToken cancellationToken)
        {
            var result = await _clientDomainService.CreateAndSendDianAsync(request.Invoices, request.token, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }
}
