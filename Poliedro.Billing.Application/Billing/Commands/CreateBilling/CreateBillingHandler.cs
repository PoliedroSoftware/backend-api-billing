using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.DomainService;
namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;
public class CreateBillingHandler(
    IDianResolutionGetByIdService _dianResolutionGetByIdService,
    ICompanyProviderGetByIdService _companyProviderGetByIdService,
    IGetProcessorBilling _createBillingFactory,
    IBillingSenderFactory _billingSenderFactory,
    IBillingResponseApi _billingResponseApi,
    IMapper mapper
    ) : IRequestHandler<CreateBillingCommand, IEnumerable<CreateBillingResultDTO>>
{
    public async Task<IEnumerable<CreateBillingResultDTO>> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
    {
        
        var resolution = await _dianResolutionGetByIdService.GetByIdAsync(request.Id, cancellationToken);

        var companyProvider = await _companyProviderGetByIdService.GetCompanyProviderByIdAsync(resolution.Value.CompanyProviderId, cancellationToken);

        var billingEntities = mapper.Map<IEnumerable<Domain.Billing.CreateBilling>>(request.Invoices);

        ICreateBilling processor = await _createBillingFactory.GetProcessorAsync(resolution.Value.ResolutionType, (ProviderType)companyProvider.ProviderId);

        IEnumerable<(Domain.Billing.CreateBilling Billing, object Output)> processedInvoices = await processor.CreateInvoicesAsync(billingEntities, resolution.Value, companyProvider, cancellationToken);

        IEnumerable<Domain.Billing.CreateBilling> billingEntitiesProcessed = processedInvoices.Select(p => p.Billing);
        IEnumerable<object> outputEntitiesProcessed = processedInvoices.Select(p => p.Output);

        
        IBillingSender sender = _billingSenderFactory.Resolve((ProviderType)companyProvider.ProviderId, resolution.Value.ResolutionType);

        var billingResults = new List<CreateBillingResultDTO>();

        
        foreach (var processed in processedInvoices)
        {
            var invoiceRequest = new PlemsiInvoiceRequest
            {
                DianResolutionEntity = resolution.Value,
                CompanyProviderEntity = companyProvider,
                Invoices = [processed.Output]
            };

            var responses = await sender.SendAsync(invoiceRequest, cancellationToken);
            var result = responses.FirstOrDefault();

            if (result is not null)
            {
                if (result.Success)
                {
                   
                    await _billingResponseApi.IBillingResponseApi(
                        new List<ApiResponseFERetailPos> { result },
                        new List<Domain.Billing.CreateBilling> { processed.Billing },
                        resolution.Value,
                        companyProvider,
                        cancellationToken
                    );
                }
                else
                {
                    Console.WriteLine($"Factura fallida: {result.Info}");
                }

                billingResults.Add(new CreateBillingResultDTO
                {
                    Status = result.Success,
                    Message = result.Success ? "Factura procesada exitosamente" : result.Info,
                    Data = null
                });
            }
        }

        return billingResults;

    }
}