using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Enums;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;

namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public class CreateBillingHandler(
    IClientDomainService _clientDomainService,
    IGetProcessorBilling _createBillingFactory,
    IBillingSenderFactory _billingSenderFactory,
    IBillingResponseApi _billingResponseApi,
    IMapper mapper
    ) : IRequestHandler<CreateBillingCommand, IEnumerable<CreateBillingDTO>>
{
    public async Task<IEnumerable<CreateBillingDTO>> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
    {
        // DTOs de entrada a CreateBilling
        var billingEntities = mapper.Map<IEnumerable<Domain.Billing.CreateBilling>>(request.Invoices);

        var client = await _clientDomainService.GetByIdAsync(request.ApiKey, cancellationToken);


        //estos datos deben entrar por factura 
        string typeResolution = client.Value.DianResolution.ResolutionType.ToString();
        ProviderType providerType = (ProviderType)client.Value.ProviderId;
        string provider = providerType.ToString();
        string Prefix = client.Value.DianResolution.Prefix;

        DateTime ExpirationDate =  client.Value.DianResolution.ExpirationDate;
        int FinalRange = client.Value.DianResolution.FinalRange;

        // obtener el proceso de construcción 
        var processor = await _createBillingFactory.GetProcessorAsync(typeResolution, provider);

        // Obtnemos un o una lista de objeto, tupla y validación de facturas
        var processedInvoices = await processor.CreateInvoicesAsync(billingEntities, ExpirationDate, FinalRange, Prefix, cancellationToken);

        var billingEntitiesProcessed = processedInvoices.Select(p => p.Billing);
        var outputEntitiesProcessed = processedInvoices.Select(p => p.Output);

        // Obtener el sender correcto
        var sender = _billingSenderFactory.Resolve(provider, typeResolution);

        // Objeto para El sender
        var InvoiceRequest = new PlemsiInvoiceRequest{ApiKey = request.ApiKey,Invoices = outputEntitiesProcessed};

        // Enviar las facturas procesadas
        var responseApi = await sender.SendAsync(InvoiceRequest, cancellationToken);

        if (responseApi.Success)
        {
           await _billingResponseApi.IBillingResponseApi(responseApi, billingEntitiesProcessed, cancellationToken);
        }
        else
        {
            throw new Exception($"Error al enviar facturas: {responseApi.Info}");
        }

        // Después haces cast o map a tus DTOs finales
        var billingDtos = mapper.Map<IEnumerable<CreateBillingDTO>>(processedInvoices);



        return billingDtos;
    }
}
