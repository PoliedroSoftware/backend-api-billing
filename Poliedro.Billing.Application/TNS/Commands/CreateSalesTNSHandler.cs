using MediatR;
using Poliedro.Billing.Application.Tns.Commands;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Application.TNS.Ports;
using Poliedro.Billing.Application.TNS.Contracts.Responses;

namespace Poliedro.Billing.Application.TNS.Commands
{
    internal class CreateSalesTNSHandler(
        ITNSGateway _clientDomainService) : IRequestHandler<CreateSalesTNSCommand, Result<ApiResponseTNS, Error>>
    {

        public async Task<Result<ApiResponseTNS, Error>> Handle(CreateSalesTNSCommand request, CancellationToken cancellationToken)
        {
            var result = await _clientDomainService.CreateAsync(request.Orders, request.codigosucursal, request.token, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!;

            return result.Value!;
        }
    }
}
