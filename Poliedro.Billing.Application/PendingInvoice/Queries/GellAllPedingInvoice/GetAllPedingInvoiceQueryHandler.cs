using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;
using Poliedro.Billing.Application.PendingInvoice.Dtos;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.PedingInvoice.DomainPedingInvoice;

namespace Poliedro.Billing.Application.PendingInvoice.Queries.GellAllPedingInvoice;

public class GetAllPedingInvoiceQueryHandler
(
    IClientDomainService clientDomainService,
    IPedingInvoiceDomainPedingInvoice pedinginvoiceDomainPedingInvoice,
    IDatabaseUtils databaseUtils,
    IMapper mapper
) : IRequestHandler<GellAllPedingInvoiceQuery, PaginatedResult<PedingInvoiceDto>>
{
    public async Task<PaginatedResult<PedingInvoiceDto>> Handle(
        GellAllPedingInvoiceQuery request, CancellationToken cancellationToken)
    {
        var client = await clientDomainService.GetByTokenAsync(
         request.Parameters.ApiKey, cancellationToken);

        if (client == null) throw new KeyNotFoundException("Cliente no encontrado.");

        if (client.IsSuccess)
        {
            var (data, totalCount) = await pedinginvoiceDomainPedingInvoice.GetAllAsync(
                 client.Value.Server, 
                 databaseUtils, 
                 client.Value.Date,
                 request.Parameters,
                 cancellationToken
        );
            var result = new PaginatedResult<PedingInvoiceDto>
                (
                Items: mapper.Map<List<PedingInvoiceDto>>(data),
                TotalCount: totalCount,
                PageNumber: request.Parameters.PageNumber,
                PageSize: request.Parameters.PageSize

                );
            return result;
        }
        else
        {
            throw new KeyNotFoundException("Cliente no encontrado.");
        }
    }

}
