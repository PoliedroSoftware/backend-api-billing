using Newtonsoft.Json;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

public class BillingSenderFE : IBillingSender
{
    public Task SendAsync(IEnumerable<object> invoices, CancellationToken cancellationToken)
    {
        //var FeInvoices = invoices.Cast<PlemiFEInvoiceDTO>().ToList();

        var jsonContent = JsonConvert.SerializeObject(invoices);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        return Task.CompletedTask;
    }
}