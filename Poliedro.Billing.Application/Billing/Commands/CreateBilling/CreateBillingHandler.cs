using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;
public class CreateBillingHandler(
    IClientDomainService _clientDomainService,
    IGetProcessorBilling _createBillingFactory,
    IBillingSenderFactory _billingSenderFactory,
    IBillingResponseApi _billingResponseApi,
    IBillingGetInfoClient _billingGetInfoClient,
    IMapper mapper
    ) : IRequestHandler<CreateBillingCommand, IEnumerable<CreateBillingResultDTO>>
{
    public async Task<IEnumerable<CreateBillingResultDTO>> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
    {
        // DTOs de entrada a CreateBilling
        var billingEntities = mapper.Map<IEnumerable<Domain.Billing.CreateBilling>>(request.Invoices);

        var clientResult = await _clientDomainService.GetByIdAsync(request.ApiKey, cancellationToken);

        if (!clientResult.IsSuccess || clientResult.Value is null)
        Console.WriteLine($"No Found Client Billing");

        ClientEntity client = clientResult.Value;
        //datos de la persistencia 
        var InfoClient = await _billingGetInfoClient.BillingInfoClient(client, cancellationToken);

        // obtener el proceso de construcción 
        ICreateBilling processor = await _createBillingFactory.GetProcessorAsync(InfoClient.TypeResolution, InfoClient.Provider);

        // Obtnemos un o una lista de objeto, tupla y validación de facturas
        IEnumerable<(Domain.Billing.CreateBilling Billing, object Output)> processedInvoices = await processor.CreateInvoicesAsync(billingEntities, InfoClient,  cancellationToken);

        IEnumerable<Domain.Billing.CreateBilling> billingEntitiesProcessed = processedInvoices.Select(p => p.Billing);
        IEnumerable<object> outputEntitiesProcessed = processedInvoices.Select(p => p.Output);

        // Obtener el sender correcto
        IBillingSender sender = _billingSenderFactory.Resolve(InfoClient.Provider, InfoClient.TypeResolution);

        // Objeto para El sender
        PlemsiInvoiceRequest InvoiceRequest = new() {ApiKey = request.ApiKey, Invoices = outputEntitiesProcessed};

        // Enviar las facturas procesadas
        List<ApiResponseFERetailPos> responseApi = await sender.SendAsync(InvoiceRequest, InfoClient, cancellationToken);

        // Persistir por factura exitosa en lugar de esperar que todas sean exitosas
        var pairedResponses = responseApi.Zip(billingEntitiesProcessed, (resp, invoice) => new { resp, invoice }).ToList();

        var successfulPairs = pairedResponses.Where(p => p.resp.Success).ToList();
        if (successfulPairs.Any())
        {
            await _billingResponseApi.IBillingResponseApi(
                successfulPairs.Select(p => p.resp).ToList(),
                successfulPairs.Select(p => p.invoice),
                cancellationToken
            );
        }

        var failedInfos = pairedResponses.Where(p => !p.resp.Success)
                                         .Select(p => p.resp.Info)
                                         .ToList();
        if (failedInfos.Any())
        {
            Console.WriteLine($"Algunas facturas fallaron al enviarse: {string.Join(" | ", failedInfos)}");
        }

        var billingResults = new List<CreateBillingResultDTO>();

        for (int i = 0; i < billingEntitiesProcessed.Count(); i++)
        {
            var result = responseApi[i];

            billingResults.Add(new CreateBillingResultDTO
            {
                Status = result.Success,
                Message = result.Success ? "Factura procesada exitosamente" : result.Info,
                Data = null 
            });
        }


        return billingResults;

    }
}