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

            var connectionString = _databaseUtils.GetConnectionString(connection.Value);

            int NumberInvoice = int.Parse(pair.invoice.Numeration);
            string CurrentlyDate = DateTime.Now.ToString();
            string LastedInvoiced = pair.invoice.Number;

            await _insertInvoiceFE.InsertInvoiceSucces(
               NumberInvoice,
                pair.resp.Data.Cude!,
                pair.resp.Data.QRCode!,
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
