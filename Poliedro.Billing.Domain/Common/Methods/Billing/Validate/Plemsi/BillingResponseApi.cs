using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Validate.Plemsi;

public class BillingResponseApi(
    IInsertInvoiceFE _insertInvoiceFE,
    IUpdateCurrentlyNumber _updateCurrentlyNumber,
    IClientDomainService _clientDomainService,
    IDatabaseUtils _databaseUtils
    ) : IBillingResponseApi
{
    public async Task IBillingResponseApi(List<ApiResponseFERetailPos> response, IEnumerable<CreateBilling> processedInvoices, CancellationToken cancellationToken)
    {
        var paired = response.Zip(processedInvoices, (resp, invoice) => new { resp, invoice });


        foreach (var pair in paired)
        {
            var customerInfo = await _clientDomainService.GetByIdAsync(pair.invoice.CustomerEntity.ApiKey, cancellationToken);

            if (customerInfo == null)
            {
                Console.WriteLine($"Error Insert Invoice Success", customerInfo);
                continue;
            }

            var connectionString = _databaseUtils.GetConnectionString(customerInfo.Value.Server);

            int NumberInvoice = int.Parse(pair.invoice.Numeration);

            await _insertInvoiceFE.InsertInvoiceSucces(
               NumberInvoice,
                pair.resp.Data.Cude!,
                pair.resp.Data.QRCode!,
               connectionString,
               customerInfo.Value.ProviderId!,
               customerInfo.Value.DianResolution.ClientBillingElectronicId,
               pair.invoice.Number
               );

            await _updateCurrentlyNumber.UpdateCurrentlyNumberAsync(
            new ParametersCurrentlyNumber(NumberInvoice, DateTime.Now.ToString(), customerInfo.Value.ResolutionId),cancellationToken
            );
        }

    }
}
