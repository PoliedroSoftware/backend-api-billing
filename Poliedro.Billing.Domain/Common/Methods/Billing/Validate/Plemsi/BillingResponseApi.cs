using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Validate.Plemsi;

public class BillingResponseApi(
    IInsertInvoiceFE _insertInvoiceFE,
    IUpdateCurrentlyNumber _updateCurrentlyNumber,
    IServerGetByIdService _serverGetByIdService,
    IDatabaseUtils _databaseUtils
    ) : IBillingResponseApi
{
    public async Task IBillingResponseApi(List<ApiResponseFERetailPos> response,
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

            apiDataPos? responseData = pair.resp.Data;

            if (responseData is null || responseData.Cude is null || responseData.QRCode is null)
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

            await _insertInvoiceFE.InsertInvoiceSucces(
               NumberInvoice,
                responseData.Cude,
                responseData.QRCode,
               connectionString,
               (ProviderType)_companyProviderEntity.ProviderId,
               _companyProviderEntity.CompanyId,
               pair.invoice.Number
               );

            await _updateCurrentlyNumber.UpdateCurrentlyNumberAsync(
            new ParametersCurrentlyNumber(NumberInvoice, CurrentlyDate, _dianResolutionEntity.ResolutionId, LastedInvoiced),cancellationToken
            );
        }

    }
}
