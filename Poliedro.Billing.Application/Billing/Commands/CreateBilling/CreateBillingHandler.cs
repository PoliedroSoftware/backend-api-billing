using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Enums;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Enums;

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


        //datos de la persistencia 
        ResolutionType typeResolution = client.Value.DianResolution.ResolutionType;
        ProviderType providerType = (ProviderType)client.Value.ProviderId;
        string provider = providerType.ToString();
        string Prefix = client.Value.DianResolution.Prefix;

        DateTime ExpirationDate =  client.Value.DianResolution.ExpirationDate;
        int FinalRange = client.Value.DianResolution.FinalRange;

        // obtener el proceso de construcción 
        ICreateBilling processor = await _createBillingFactory.GetProcessorAsync(typeResolution, provider);

        // Obtnemos un o una lista de objeto, tupla y validación de facturas
        IEnumerable<(Domain.Billing.CreateBilling Billing, object Output)> processedInvoices = await processor.CreateInvoicesAsync(billingEntities, ExpirationDate, FinalRange, Prefix, cancellationToken);

        IEnumerable<Domain.Billing.CreateBilling> billingEntitiesProcessed = processedInvoices.Select(p => p.Billing);
        IEnumerable<object> outputEntitiesProcessed = processedInvoices.Select(p => p.Output);

        // Obtener el sender correcto
        IBillingSender sender = _billingSenderFactory.Resolve(provider, typeResolution);

        // Objeto para El sender
        PlemsiInvoiceRequest InvoiceRequest = new() {ApiKey = request.ApiKey,Invoices = outputEntitiesProcessed};

        // Enviar las facturas procesadas
        ApiResponseFERetailPos responseApi = await sender.SendAsync(InvoiceRequest, cancellationToken);

        if (responseApi.Success)
        {
           await _billingResponseApi.IBillingResponseApi(responseApi, billingEntitiesProcessed, cancellationToken);
        }
        else
        {
            throw new Exception($"Error al enviar facturas: {responseApi.Info}");
        }

        // Después haces cast o map a tus DTOs finales
        IEnumerable<CreateBillingDTO> billingDtos = mapper.Map<IEnumerable<CreateBillingDTO>>(processedInvoices);



        return billingDtos;
    }
}
