using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.PdfInvoice.Queries;

namespace Poliedro.Billing.Api.Endpoints.v1.PdfInvoice;

public static class PdfInvoiceEndpoints
{
    public static RouteGroupBuilder MapPdfInvoiceEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/pdfinvoice/pdf/{id}", GetPdfInvoice)
            .WithName("GetPdfInvoice")
            .WithTags("PdfInvoice")
            .WithSummary("Get PDF invoice")
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> GetPdfInvoice(
        HttpContext context,
        IMediator mediator,
        string id,
        [FromHeader(Name = "Authorization")] string authorization
        )
    {
        try
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Results.Unauthorized();
            }

            string token = authorization.ToString()["Bearer ".Length..];

            byte[] pdfBytes = await mediator.Send(new GetPdfInvoiceQuery(id, token));

            if (pdfBytes == null || pdfBytes.Length == 0)
                return Results.NotFound();

            return Results.File(pdfBytes, "application/pdf", "factura.pdf");
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
