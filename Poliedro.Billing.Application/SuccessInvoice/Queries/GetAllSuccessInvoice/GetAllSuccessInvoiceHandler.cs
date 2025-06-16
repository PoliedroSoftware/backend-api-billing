using MediatR;
using AutoMapper;
using Poliedro.Billing.Application.SuccessInvoice.Dtos;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;
namespace Poliedro.Billing.Application.SuccessInvoice.Queries.GetAllSuccessInvoice;
public class GetAllSuccessInvoiceHandler(
    ISuccessInvoiceRepository successInvoiceRepository,
    IMapper mapper) : IRequestHandler<GetAllSuccessInvoiceQuery, IEnumerable<SuccessInvoiceDto>>
{
    public async Task<IEnumerable<SuccessInvoiceDto>> Handle(
        GetAllSuccessInvoiceQuery request,
        CancellationToken cancellationToken)
    {
        var successInvoices = await successInvoiceRepository
            .GetAllInvoice(request.Parameters);
        return mapper.Map<IEnumerable<SuccessInvoiceDto>>(successInvoices);
    }
}