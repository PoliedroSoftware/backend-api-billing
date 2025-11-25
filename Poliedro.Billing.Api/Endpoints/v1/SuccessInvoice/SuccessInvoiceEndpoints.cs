using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Application.SuccessInvoice.Dtos;
using Poliedro.Billing.Application.SuccessInvoice.Queries.GetAllSuccessInvoice;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;

namespace Poliedro.Billing.Api.Endpoints.v1.SuccessInvoice;

public static class SuccessInvoiceEndpoints
{
    public static RouteGroupBuilder MapSuccessInvoiceEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllSuccessInvoice)
            .WithName("GetAllSuccessInvoices")
            .WithTags("SuccessInvoice")
            .WithSummary("Get all success invoices")
            .WithDescription("Retrieves a paginated list of success invoices based on search criteria")
            .Produces<SuccessInvoiceDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAllSuccessInvoice(
        HttpContext context,
        string order = "desc",
        int page = 1,
        int perPage = 10,
        IMediator mediator = null!)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authorization) || 
            string.IsNullOrEmpty(authorization) || 
            !authorization.ToString().StartsWith("Bearer "))
        {
            return Results.Unauthorized();
        }
        
        string token = authorization.ToString()["Bearer ".Length..];

        var parameters = new SuccessInvoiceQueryParameters(
            Token: token,
            Order: order,
            Page: page,
            PerPage: perPage
        );

        var successInvoices = await mediator.Send(new GetAllSuccessInvoiceQuery(parameters));
        return Results.Ok(successInvoices);
    }
}
