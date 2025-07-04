using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.GetInvoice.Dtos;
using Poliedro.Billing.Domain.GetInvoice.DomainGetInvoice;

namespace Poliedro.Billing.Application.GetInvoice.Queries;

public class GetByIdGetInvoiceQueryHandler(IGetInvoiceDomainGetInvoice getInvoiceDomainGetInvoice, IMapper mapper)
    : IRequestHandler<GetByIdGetInvoiceQuery, GetInvoiceDto>
{
    public async Task<GetInvoiceDto> Handle(GetByIdGetInvoiceQuery request, CancellationToken cancellationToken)
        => mapper.Map<GetInvoiceDto>(await getInvoiceDomainGetInvoice.GetByIdAsync(request.cufe, request.token, cancellationToken));
}
