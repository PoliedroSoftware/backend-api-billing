using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail;

namespace Poliedro.Billing.Api.Endpoints.v1.FERetail;

public static class FERetailEndpoints
{
    public static RouteGroupBuilder MapFERetailEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .WithName("CreateFERetail")
            .WithTags("FERetail")
            .WithSummary("Create new FERetail")
            .Produces<CreateFERetailCommand>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> Create(
        CreateFERetailCommand createFERetailCommand,
        IMediator mediator)
    {
        var result = await mediator.Send(createFERetailCommand);
        return result.Match(onSuccess => TypedResults.Ok());
    }
}
