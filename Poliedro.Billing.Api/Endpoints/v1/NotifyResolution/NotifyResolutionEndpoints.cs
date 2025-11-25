using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.NotifyResolution.Dtos;
using Poliedro.Billing.Application.NotifyResolution.Queries.GetNotifyResolution;
using Poliedro.Billing.Domain.NotifyResolution.Ports;

namespace Poliedro.Billing.Api.Endpoints.v1.NotifyResolution;

public static class NotifyResolutionEndpoints
{
    public static RouteGroupBuilder MapNotifyResolutionEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetNotifyResolution)
            .WithName("GetNotifyResolution")
            .WithTags("NotifyResolution")
            .WithSummary("Get notify resolution by Bearer token")
            .WithDescription("Retrieves notify resolution based on Bearer token")
            .Produces<NotifyResolutionDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetNotifyResolution(
        HttpContext context,
        IMediator mediator)
    {
        var token = TokenHelper.ExtractBearerToken(context.Request);

        if (string.IsNullOrEmpty(token))
            return Results.Json(
                "Authorization header is missing or invalid.",
                statusCode: StatusCodes.Status401Unauthorized);

        var parameters = new NotifyResolutionParameters(ApiKey: token);

        var notifyResolution = await mediator.Send(new GetNotifyResolutionByIdQuery(parameters));

        if (notifyResolution == null)
        {
            var notFoundResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Notify resolution not found."
            );
            return Results.NotFound(notFoundResponse);
        }

        var successResponse = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            data: notifyResolution,
            message: "Successful query"
        );

        return Results.Ok(successResponse);
    }
}
