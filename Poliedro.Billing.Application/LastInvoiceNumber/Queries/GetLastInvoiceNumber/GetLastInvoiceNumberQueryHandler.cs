using MediatR;
using Poliedro.Billing.Application.Client.Errors;
using Poliedro.Billing.Application.LastInvoiceNumber.Dtos;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.LastInvoiceNumber.Queries.GetLastInvoiceNumber;

public class GetLastInvoiceNumberQueryHandler(
    IClientGetByIdService clientGetByIdService,
    IBillingGetInfoClient billingGetInfoClient,
    IGetLastInvoiceBilling getLastInvoiceBilling,
    IInvoiceLastPos invoiceLastPos
    ) : IRequestHandler<GetLastInvoiceNumberQuery, Result<LastInvoiceNumberDto, Error>>
{
    public async Task<Result<LastInvoiceNumberDto, Error>> Handle(GetLastInvoiceNumberQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ApiKey))
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundByApiKeyException();

        var clientResult = await clientGetByIdService.GetByIdAsync(request.ApiKey, cancellationToken);
        
        if (!clientResult.IsSuccess)
            return ClientBillingElectronicErrorBuilder.ClientBillingNotFoundByApiKeyException();

        var clientEntity = clientResult.Value;
        var clientInfo = await billingGetInfoClient.BillingInfoClient(clientEntity, cancellationToken);

        int nextInvoiceNumber;

        try
        {
            if (clientInfo.TypeResolution == ResolutionType.FE)
            {
                nextInvoiceNumber = await getLastInvoiceBilling.GetLastInvoiceNumberAsync(clientInfo, cancellationToken);
            }
            else if (clientInfo.TypeResolution == ResolutionType.POS)
            {
                nextInvoiceNumber = await invoiceLastPos.GetInvoiceLastAsync(clientInfo, cancellationToken);
            }
            else
            {
                return ClientBillingElectronicErrorBuilder.InvalidResolutionTypeException();
            }
        }
        catch (Exception ex)
        {
            return ClientBillingElectronicErrorBuilder.GetLastInvoiceNumberException(ex.Message);
        }

        var result = new LastInvoiceNumberDto
        {
            NextInvoiceNumber = nextInvoiceNumber,
            Prefix = clientInfo.Prefix,
            ResolutionNumber = clientInfo.ResolucionNumber,
            CurrentlyNumber = clientInfo.CurrentlyNumber,
            FinalRange = clientInfo.FinalRange,
            ExpirationDate = clientInfo.ExpirationDate
        };

        return result;
    }
}
