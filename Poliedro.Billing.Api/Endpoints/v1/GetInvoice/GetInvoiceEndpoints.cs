using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.GetInvoice.Queries;

namespace Poliedro.Billing.Api.Endpoints.v1.GetInvoice;

public static class GetInvoiceEndpoints
{
    public static RouteGroupBuilder MapGetInvoiceEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetById)
            .WithName("GetInvoiceByCufe")
            .WithTags("GetInvoice")
            .WithSummary("Get invoice by CUFE")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetById(
        HttpContext context,
        string cufe,
        IMediator mediator)
    {
        if (string.IsNullOrWhiteSpace(cufe))
        {
            return Results.BadRequest(ResponseApiService.Response(StatusCodes.Status400BadRequest, "CUFE is required"));
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authorization) || string.IsNullOrWhiteSpace(authorization))
        {
            return Results.Json(
                ResponseApiService.Response(StatusCodes.Status401Unauthorized, "Authorization header is required"),
                statusCode: StatusCodes.Status401Unauthorized);
        }

        if (!authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Json(
                ResponseApiService.Response(StatusCodes.Status401Unauthorized, "Invalid authorization format. Use 'Bearer {token}'"),
                statusCode: StatusCodes.Status401Unauthorized);
        }

        string token = authorization.ToString()["Bearer ".Length..].Trim();

        if (string.IsNullOrWhiteSpace(token))
        {
            return Results.Json(
                ResponseApiService.Response(StatusCodes.Status401Unauthorized, "Token cannot be empty"),
                statusCode: StatusCodes.Status401Unauthorized);
        }

        try
        {
            var data = await mediator.Send(new GetByIdGetInvoiceQuery(cufe, token));

            if (data is null)
            {
                return Results.NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound, "Invoice not found"));
            }

            return Results.Ok(ResponseApiService.Response(StatusCodes.Status200OK, data));
        }
        catch (Exception)
        {
            return Results.Json(
                ResponseApiService.Response(StatusCodes.Status500InternalServerError, "An error occurred processing your request"),
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
