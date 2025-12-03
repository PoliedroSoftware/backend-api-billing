using MediatR;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.LastInvoiceNumber.Dtos;
using Poliedro.Billing.Application.LastInvoiceNumber.Queries.GetLastInvoiceNumber;

namespace Poliedro.Billing.Api.Endpoints.v1.LastInvoiceNumber;

public static class LastInvoiceNumberEndpoints
{
    public static RouteGroupBuilder MapLastInvoiceNumberEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetLastInvoiceNumber)
            .WithName("GetLastInvoiceNumber")
            .WithTags("LastInvoiceNumber")
            .WithSummary("Get last invoice number + 1")
            .WithDescription("Returns the next available invoice number for the authenticated client")
            .Produces<LastInvoiceNumberDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetLastInvoiceNumber(
        HttpContext context,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var token = TokenHelper.ExtractBearerToken(context.Request);
        if (string.IsNullOrEmpty(token))
            return Results.Unauthorized();

        var query = new GetLastInvoiceNumberQuery(token);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            var errorResponse = ResponseApiService.Response(
                statusCode: (int)result.Error.HttpStatusCode,
                message: result.Error.Description,
                data: (object?)null
            );
            return Results.Json(errorResponse, statusCode: (int)result.Error.HttpStatusCode);
        }

        var response = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            message: "Last invoice number retrieved successfully.",
            data: result.Value
        );
        return Results.Ok(response);
    }
}
