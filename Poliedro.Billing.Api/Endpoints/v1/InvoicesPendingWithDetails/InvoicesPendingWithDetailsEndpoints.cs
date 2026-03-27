using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Application.Common.Features;
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
            .WithSummary("Get pending invoices with details by client id and provider type")
            .WithDescription("Retrieves pending invoices with details based on ClientID and ProviderType in the request body")
            .Produces<IEnumerable<CreateBillingDTO>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAllAsync(
        InvoicesPendingWithDetailsQuery request,
        IMediator mediator)
    {
        if (request is null)
            return Results.BadRequest(ResponseApiService.Response(StatusCodes.Status400BadRequest, "Request body is required"));

        var invoicesPendingWithDetails = await mediator.Send(request);

        return Results.Json(
            ResponseApiService.Response(StatusCodes.Status201Created, invoicesPendingWithDetails),
            statusCode: StatusCodes.Status201Created);
    }
}
