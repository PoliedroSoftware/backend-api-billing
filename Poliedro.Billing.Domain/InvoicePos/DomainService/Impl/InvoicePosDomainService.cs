using Poliedro.Billing.Domain.InvoicePos.Entities;
using Poliedro.Billing.Domain.InvoicePos.Ports;

namespace Poliedro.Billing.Domain.InvoicePos.DomainService.Impl;

public class InvoicePosDomainService : IInvoicePosDomainService
{
    private readonly IInvoicePosRepository _invoicePosRepository;

    public InvoicePosDomainService(IInvoicePosRepository invoicePosRepository)
    {
        _invoicePosRepository = invoicePosRepository;
    }

    public async Task<IEnumerable<InvoicePosEntity>> GetAllAsync(string? prefix, CancellationToken cancellationToken)
    {
        return await _invoicePosRepository.GetAllAsync(prefix, cancellationToken);
    }
}
