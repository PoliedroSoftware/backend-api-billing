using MediatR;
using Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;
namespace Polideiro.Billing.Application.InvoiceDetailElectronic.Queries.GetAllInvoiceDetailElectronic;


public record GetAllInvoiceDetailElectronicQuery(
    InvoiceElectronicParameters Parameters
) : IRequest<PaginatedResult<InvoiceElectronicDto>>;