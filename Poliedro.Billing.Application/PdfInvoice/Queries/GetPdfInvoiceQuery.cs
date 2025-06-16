using MediatR;
namespace Poliedro.Billing.Application.PdfInvoice.Queries;

public record GetPdfInvoiceQuery(string PdfInvoiceId, string BearerToken) : IRequest<byte[]>;

