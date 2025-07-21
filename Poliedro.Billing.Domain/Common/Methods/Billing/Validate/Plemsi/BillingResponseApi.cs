using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Validate.Plemsi;

public class BillingResponseApi(
    IInsertInvoiceFE _insertInvoiceFE,
    IUpdateCurrentlyNumber _updateCurrentlyNumber
    ) : IBillingResponseApi
{
    public async Task IBillingResponseApi(ApiResponseFERetailPos response, IEnumerable<object> processedInvoices, CancellationToken cancellationToken)
    {
        foreach (var item in processedInvoices)
        {
            //await _insertInvoice.InsertInvoiceSucces(
            //    item.InvoiceId,
            //    response.Data.Cude!,
            //    response.Data.QRCode!,
            //    //client.ConnectionString,
            //    //client.ProviderId,
            //    //client.ClientBillingElectronicId,
            //    item);

            //await _updateCurrentlyNumber.UpdateCurrentlyNumberAsync(
            //    new ParametersCurrentlyNumber(item.InvoiceId, DateTime.Now.ToString(), client.ResolutionId),
            //    cancellationToken);
        }

    }
}
