using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Common.Dtos;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

namespace Poliedro.Billing.Api.Endpoints.v1.InvoicesPendingWithDetails;

public static class InvoicesPendingWithDetailsEndpoints
{
    public static RouteGroupBuilder MapInvoicesPendingWithDetailsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllAsync)
            .WithName("GetInvoicesPendingWithDetails")
            .WithTags("InvoicesPendingWithDetails")
            .WithSummary("Get pending invoices with details by Bearer token")
            .WithDescription("Retrieves pending invoices with details based on Bearer token")
            .Produces<PagedResponseDto<CreateBillingDTO>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAllAsync(
        HttpContext context,
        IMediator mediator,
        int page = 1,
        int pageSize = 10)
    {
        var token = TokenHelper.ExtractBearerToken(context.Request);
        if (string.IsNullOrEmpty(token))
            return Results.Json("Authorization header is missing or invalid.",
                statusCode: StatusCodes.Status401Unauthorized);

        var result = await mediator.Send(
            new InvoicesPendingWithDetailsQuery(ApiKey: token, Page: page, PageSize: pageSize));

        return Results.Ok(result);
    }
}
