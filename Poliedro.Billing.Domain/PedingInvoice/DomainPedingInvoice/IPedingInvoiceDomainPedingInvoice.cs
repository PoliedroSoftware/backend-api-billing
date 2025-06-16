
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;
using Poliedro.Billing.Domain.PedingInvoice.Entities;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.PedingInvoice.DomainPedingInvoice;

public interface IPedingInvoiceDomainPedingInvoice
{
    Task<(IEnumerable<PedingInvoiceEntity> Data, int TotalCount )> GetAllAsync(
        ServerEntity server,
        IDatabaseUtils databaseUtils,
        DateTime date,
        InvoiceElectronicParameters paginationParams,
        CancellationToken cancellationToken
        );
}
