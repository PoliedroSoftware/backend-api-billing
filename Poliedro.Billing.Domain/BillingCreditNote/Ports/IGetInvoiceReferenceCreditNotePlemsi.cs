
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.BillingCreditNote.Ports;

public interface IGetInvoiceReferenceCreditNotePlemsi
{
    Task<InvoiceReferenceEntity> GetInvoiceReferenceCreditNotePlemsiAsync(
        string Verify,
        ServerEntity Server,
        CancellationToken cancellationToken);
}
