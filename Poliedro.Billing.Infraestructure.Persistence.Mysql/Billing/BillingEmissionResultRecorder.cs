using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Billing;

public sealed class BillingEmissionResultRecorder(
    IInvoiceEmissionStore _invoiceEmissionStore,
    IUpdateCurrentlyNumber _updateCurrentlyNumber,
    IServerGetByIdService _serverGetByIdService,
    IDatabaseUtils _databaseUtils
    ) : IBillingEmissionResultRecorder
{
    public async Task RecordAsync(IReadOnlyList<BillingProviderResponse> response,
        IEnumerable<CreateBilling> _processedInvoices,
        DianResolutionEntity _dianResolutionEntity,
        CompanyProviderEntity _companyProviderEntity,
        CancellationToken cancellationToken)
    {
        var paired = response.Zip(_processedInvoices, (resp, invoice) => new { resp, invoice });


        foreach (var pair in paired)
        {
            var connection = await _serverGetByIdService.GetByIdAsync(_companyProviderEntity.ServiceId, cancellationToken);

            if (connection.Value is null)
            {
                throw new InvalidOperationException(
                    $"No se encontró el servidor {_companyProviderEntity.ServiceId} del company provider {_companyProviderEntity.CompanyProviderId}.");
            }

            var connectionString = _databaseUtils.GetConnectionString(connection.Value);

            if (string.IsNullOrWhiteSpace(pair.resp.Cude) || string.IsNullOrWhiteSpace(pair.resp.QrCode))
            {
                throw new InvalidOperationException(
                    $"La respuesta del proveedor para la factura {pair.invoice.Number} no incluye Cude/QRCode.");
            }

            if (!int.TryParse(pair.invoice.Numeration, out int NumberInvoice))
            {
                throw new InvalidOperationException(
                    $"La numeración '{pair.invoice.Numeration}' de la factura {pair.invoice.Number} no es un número válido.");
            }

            string CurrentlyDate = DateTime.Now.ToString();
            string LastedInvoiced = pair.invoice.Number;

            await _invoiceEmissionStore.RecordSuccessAsync(
               NumberInvoice,
                pair.resp.Cude,
                pair.resp.QrCode,
               connectionString,
               _companyProviderEntity.ProviderId,
               _companyProviderEntity.CompanyId,
               pair.invoice.Number,
               cancellationToken
               );

            await _updateCurrentlyNumber.UpdateCurrentlyNumberAsync(
            new ParametersCurrentlyNumber(NumberInvoice, CurrentlyDate, _dianResolutionEntity.ResolutionId, LastedInvoiced),cancellationToken
            );
        }

    }
}
