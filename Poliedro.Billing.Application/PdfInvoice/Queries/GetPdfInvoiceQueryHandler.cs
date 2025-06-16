using MediatR;
using Poliedro.Billing.Domain.PdfInvoice.Service;

namespace Poliedro.Billing.Application.PdfInvoice.Queries;

public class GetPdfInvoiceQueryHandler(IPdfInvoiceService pdfInvoiceService)
    : IRequestHandler<GetPdfInvoiceQuery, byte[]>
{
    public async Task<byte[]> Handle(GetPdfInvoiceQuery request, CancellationToken cancellationToken)
    {
        return await pdfInvoiceService.GetPdfBytes(request.PdfInvoiceId, request.BearerToken);
    }
}


