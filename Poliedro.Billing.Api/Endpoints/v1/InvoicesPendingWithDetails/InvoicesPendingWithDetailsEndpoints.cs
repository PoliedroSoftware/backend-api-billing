using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

namespace Poliedro.Billing.Api.Endpoints.v1.InvoicesPendingWithDetails;

public static class InvoicesPendingWithDetailsEndpoints
{
    public static RouteGroupBuilder MapInvoicesPendingWithDetailsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", GetAllAsync)
            .WithName("GetInvoicesPendingWithDetails")
            .WithTags("InvoicesPendingWithDetails")
            .WithSummary("Get pending invoices with details by Id Client and Provider")
            .WithDescription("Retrieves pending invoices with details based on Id Client and Provider")
            .Produces<IEnumerable<CreateBillingDTO>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAllAsync(
        HttpContext context,
        IMediator mediator,
        [FromBody] InvoicesPendingWithDetailsQuery query)
    {

        int Id = query.Id;


        IEnumerable<CreateBillingDTO> invoicesPendingWithDetails = await mediator.Send(new InvoicesPendingWithDetailsQuery(Id: Id));

        return Results.Ok(invoicesPendingWithDetails);
    }
}
