using MediatR;
using Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;
using Poliedro.Billing.Application.PendingInvoice.Dtos;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;

namespace Poliedro.Billing.Application.PendingInvoice.Queries.GellAllPedingInvoice;

public record GellAllPedingInvoiceQuery(
    InvoiceElectronicParameters Parameters
    ) : IRequest<PaginatedResult<PedingInvoiceDto>>;
