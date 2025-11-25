using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.Tns.Commands;

namespace Poliedro.Billing.Api.Endpoints.v1.Tns;

public static class TnsEndpoints
{
    public static RouteGroupBuilder MapTnsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .WithName("CreateSalesTns")
            .WithTags("Tns")
            .WithSummary("Create new Billings TNS")
            .Produces<CreateSalesTNSCommand>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> Create(
        HttpContext context,
        CreateSalesTNSCommand command,
        IMediator mediator)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return Results.Unauthorized();
        }

        var updatedCommand = command with { token = authHeader.ToString().Replace("Bearer ", string.Empty) };

        if (command == null || command.Orders.Count == 0)
        {
            return Results.BadRequest("The order list is empty or null.");
        }

        var result = await mediator.Send(updatedCommand);

        return result.Match(
            onSuccess => TypedResults.Ok(onSuccess),
            onFailure => TypedResults.BadRequest(onFailure)
        );
    }
}
