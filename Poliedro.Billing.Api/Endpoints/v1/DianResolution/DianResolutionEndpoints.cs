using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.DeleteDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.UpdateDianResolution;
using Poliedro.Billing.Application.DianResolution.Dtos;
using Poliedro.Billing.Application.DianResolution.Queries.GetAllDianResolution;
using Poliedro.Billing.Application.DianResolution.Queries.GetDianResolutionById;

namespace Poliedro.Billing.Api.Endpoints.v1.DianResolution;

public static class DianResolutionEndpoints
{
    public static RouteGroupBuilder MapDianResolutionEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .WithName("CreateDianResolution")
            .WithTags("DianResolution")
            .WithSummary("Create new Dian Resolution")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetAll)
            .WithName("GetAllDianResolutions")
            .WithTags("DianResolution")
            .WithSummary("Get all Dian Resolution")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetById)
            .WithName("GetDianResolutionById")
            .WithTags("DianResolution")
            .WithSummary("Get Dian Resolution")
            .Produces<DianResolutionDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPatch("/", Update)
            .WithName("UpdateDianResolution")
            .WithTags("DianResolution")
            .WithSummary("Update dian resolution")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", Delete)
            .WithName("DeleteDianResolution")
            .WithTags("DianResolution")
            .WithSummary("Delete Dian Resolution")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> Create(
        CreateDianResolutionCommand createDianResolutionCommand,
        IMediator mediator)
    {
        var result = await mediator.Send(createDianResolutionCommand);
        return result.Match(onSuccess => TypedResults.NoContent());
    }

    private static async Task<IResult> GetAll(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllDianResolutionQuery());
        return result.Match(onSuccess => TypedResults.Ok(result.Value));
    }

    private static async Task<IResult> GetById(int id, IMediator mediator)
    {
        var getDianResolutionQuery = new GetDianResolutionByIdQuery { Id = id };
        var result = await mediator.Send(getDianResolutionQuery);
        return result.Match(onSuccess => TypedResults.Ok(result.Value));
    }

    private static async Task<IResult> Update(
        UpdateDianResolutionCommand updateDianResolutionCommand,
        IMediator mediator)
    {
        var result = await mediator.Send(updateDianResolutionCommand);
        return result.Match(onSuccess => TypedResults.NoContent());
    }

    private static async Task<IResult> Delete(int id, IMediator mediator)
    {
        var deleteDianResolutionCommand = new DeleteDianResolutionCommand { Id = id };
        var result = await mediator.Send(deleteDianResolutionCommand);
        return result.Match(onSuccess => TypedResults.NoContent());
    }
}
