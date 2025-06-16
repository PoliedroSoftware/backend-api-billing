using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Entities;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;

public interface IInvoiceElectronicRepository
{
    Task<(IEnumerable<InvoiceElectronic> Data, int TotalCount)> GetAllInvoiceElectronicWithDetails(
        ServerEntity server,
        IDatabaseUtils databaseUtils,
        InvoiceElectronicParameters parameters,
        CancellationToken cancellationToken);
}