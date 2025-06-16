using AutoMapper;
using MediatR;
using Polideiro.Billing.Application.InvoiceDetailElectronic.Queries.GetAllInvoiceDetailElectronic;
using Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;

namespace Poliedro.Billing.Application.InvoiceDetailElectronic.Queries.GetAllInvoiceDetailElectronic;

public class InvoiceDetailElectronicHandler(
    IClientDomainService clientDomainService,
    IMapper mapper,
    IInvoiceElectronicRepository invoiceElectronicRepository,
    IDatabaseUtils databaseUtils
    ) : IRequestHandler<GetAllInvoiceDetailElectronicQuery, PaginatedResult<InvoiceElectronicDto>>
{
    public async Task<PaginatedResult<InvoiceElectronicDto>> Handle(
        GetAllInvoiceDetailElectronicQuery request, CancellationToken cancellationToken)
    {
        var client = await clientDomainService.GetByTokenAsync(
            request.Parameters.ApiKey, cancellationToken);

        if (client == null) throw new KeyNotFoundException("Cliente no encontrado.");
        

        if (client.IsSuccess)
        {
            var (data, totalCount) = await invoiceElectronicRepository.GetAllInvoiceElectronicWithDetails(
                client.Value.Server, 
                databaseUtils,
                request.Parameters,
                cancellationToken);

            var result = new PaginatedResult<InvoiceElectronicDto>
        (
            Items: mapper.Map<List<InvoiceElectronicDto>>(data),

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
