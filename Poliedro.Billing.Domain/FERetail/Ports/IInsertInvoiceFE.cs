using Poliedro.Billing.Domain.Client.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poliedro.Billing.Domain.FERetail.Ports
{
    public interface IInsertInvoiceFE
    {
       Task InsertInvoiceSucces(
       int invoice,
       string? cude,
       string? QRCode,
       string connectionString,
       ProviderType providerType,
       int ClientBillingElectronicId,
       string invoiceResolution);
    }
}
