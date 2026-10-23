using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;

using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.Responses;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.FE;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.POS;
using AutoMapper;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing;

/// <summary>Translates the application provider contract to the existing Plemsi transport contract.</summary>
public sealed class PlemsiBillingProviderAdapter(
    IBillingSenderFactory senderFactory,
    IGetLastInvoiceBilling feNumbering,
    IInvoiceLastPos posNumbering) : IBillingProviderAdapter
{
    private static readonly IMapper Mapper = new MapperConfiguration(configuration =>
        configuration.AddProfile<PlemsiBillingMapperProfile>(),
        Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance).CreateMapper();

    public int ProviderId => 1;
    public string ProviderKey => "plemsi";

    public ProviderConfigurationValidation ValidateConfiguration(CompanyProviderEntity companyProvider) =>
        string.IsNullOrWhiteSpace(companyProvider.ApiKey)
            ? ProviderConfigurationValidation.Invalid("El company provider no tiene una ApiKey configurada para Plemsi.")
            : ProviderConfigurationValidation.Valid();

    public Task<int> GetLastNumberAsync(
        DianResolutionEntity resolution,
        CompanyProviderEntity companyProvider,
        CancellationToken cancellationToken) => resolution.ResolutionType switch
        {
            ResolutionType.FE => feNumbering.GetLastInvoiceNumberAsync(resolution, companyProvider, cancellationToken),
            ResolutionType.POS => posNumbering.GetInvoiceLastAsync(resolution, companyProvider, cancellationToken),
            _ => throw new InvalidOperationException($"Plemsi does not support document type '{resolution.ResolutionType}'.")
        };

    public async Task<IReadOnlyList<BillingProviderResponse>> SendAsync(
        BillingProviderSubmission submission,
        CancellationToken cancellationToken)
    {
        var sender = senderFactory.Resolve(ProviderId, submission.DocumentType);
        var providerDocuments = submission.DocumentType switch
        {
            ResolutionType.FE => submission.PreparedDocuments
                .Select(document => (object)Mapper.Map<SenderRequestFEDTO>(document.ElectronicInvoice
                    ?? throw new InvalidOperationException("Se esperaba una factura electrónica preparada."))).ToArray(),
            ResolutionType.POS => submission.PreparedDocuments
                .Select(document => (object)Mapper.Map<InvoiceRequestPosDto>(document.PointOfSaleInvoice
                    ?? throw new InvalidOperationException("Se esperaba una factura POS preparada."))).ToArray(),
            _ => throw new InvalidOperationException($"Plemsi does not support document type '{submission.DocumentType}'.")
        };
        var response = await sender.SendAsync(new PlemsiInvoiceRequest
        {
            DianResolutionEntity = submission.Resolution,
            CompanyProviderEntity = submission.CompanyProvider,
            Invoices = providerDocuments
        }, cancellationToken);

        return response.Select(Map).ToArray();
    }

    private static BillingProviderResponse Map(ApiResponseFERetailPos response) =>
        new(response.Code, response.Success, response.Info, response.Data?.Cude, response.Data?.QRCode, response.Data?.FootNote);
}
