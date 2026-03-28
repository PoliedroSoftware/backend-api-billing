using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Common.Dtos;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

public class InvoicesPendingWithDetailsHandler(
    IClientDomainService clientDomainService,
    IMapper mapper,
    IDatabaseUtils databaseUtils,
    IInvoicesPendingWithDetailsStrategyFactory _strategyFactory
    ) : IRequestHandler<InvoicesPendingWithDetailsQuery, PagedResponseDto<CreateBillingDTO>>
{
    public async Task<PagedResponseDto<CreateBillingDTO>> Handle(
        InvoicesPendingWithDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var client = await clientDomainService.GetByIdAsync(request.ApiKey, cancellationToken);

        var repository = _strategyFactory.GetStrategy(
            (Domain.InvoicesPendingWithDetails.Enums.ResolutionType)client.Value.DianResolution.ResolutionType);

        var raw = await repository.GetAllInvoicePendingWithDetails(
            client.Value.Server,
            client.Value,
            databaseUtils,
            cancellationToken);

        var mapped = mapper.Map<IEnumerable<CreateBillingDTO>>(raw).ToList();

        var total = mapped.Count;
        var paged = mapped
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResponseDto<CreateBillingDTO>
        {
            Success = true,
            StatusCode = 200,
            Message = "Invoices retrieved successfully",
            Data = paged,
            Meta = new MetaDto
            {
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize)
            }
        };
    }
}
