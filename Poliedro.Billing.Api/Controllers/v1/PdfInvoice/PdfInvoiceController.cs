using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.PdfInvoice.Queries;

namespace Poliedro.Billing.Api.Controllers.v1.PdfInvoice;

[Route("api/billing/pdfinvoice")]
[ApiController]
public class PdfInvoiceController(IMediator mediator) : ControllerBase
{
    [HttpGet("pdf/{id}")]
    public async Task<IActionResult> GetPdfInvoice(string id, [FromHeader(Name = "Authorization")] string authorization)
    {
        try
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized("Bearer token is required.");
            }

            string token = authorization["Bearer ".Length..];

            byte[] pdfBytes = await mediator.Send(new GetPdfInvoiceQuery(id, token));

            if (pdfBytes == null || pdfBytes.Length == 0)
                return NotFound("PDF not found or empty.");

            return File(pdfBytes, "application/pdf", "factura.pdf");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
