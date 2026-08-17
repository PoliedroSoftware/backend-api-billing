using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

namespace Poliedro.Billing.Api.Endpoints.v1.InvoicesPendingWithDetails;

public static class InvoicesPendingWithDetailsEndpoints
{
    public static RouteGroupBuilder MapInvoicesPendingWithDetailsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", GetByResolutionId)
            .WithName("GetInvoicesPendingById")
            .WithTags("InvoicesPendingWithDetails")
            .WithSummary("Get pending invoices with details by Id")
            .WithDescription("Retrieves pending invoices with details based on Id")
            .Produces<InvoicesPendingWithDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetByResolutionId(
        int id,
        IMediator mediator,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("InvoicesPendingWithDetails");

        try
        {
            var result = await mediator.Send(new InvoicesPendingWithDetailsQuery(id));

            if (result is null)
            {
                return Results.NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));
            }

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving pending invoices for resolution {ResolutionId}", id);
            return Results.Json(
                ResponseApiService.Response(StatusCodes.Status500InternalServerError, "An error occurred processing your request"),
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}