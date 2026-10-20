using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.Server.Commands.CreateServer;
using Poliedro.Billing.Application.Server.Commands.UpdateServer;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Application.Server.Errors;
using Poliedro.Billing.Application.Server.Queries.GellAllServer;
using Poliedro.Billing.Application.Server.Queries.GetServerById;
using Poliedro.Billing.Domain.Common.Pagination;

namespace Poliedro.Billing.Api.Endpoints.v1.Server;

public static class ServerEndpoints
{
    public static RouteGroupBuilder MapServerEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll)
            .WithName("GetAllServers")
            .WithTags("Server")
            .WithSummary("Get all servers")
            .WithDescription("Retrieves all Server records")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetById)
            .WithName("GetServerById")
            .WithTags("Server")
            .WithSummary("Get Server")
            .Produces<ServerDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
            .WithName("CreateServer")
            .WithTags("Server")
            .WithSummary("Create new Server")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/", Update)
            .WithName("UpdateServer")
            .WithTags("Server")
            .WithSummary("Update an existing Server")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAll(
        IMediator mediator,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var data = await mediator.Send(new GellAllServerQuery(new PaginationParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        }));

        if (data is null)
        {
            return Results.Json(
                ResponseApiService.Response(StatusCodes.Status404NotFound),
                statusCode: StatusCodes.Status404NotFound);
        }

        return Results.Ok(ResponseApiService.Response(StatusCodes.Status200OK, data));
    }

    private static async Task<IResult> GetById(
        int id,
        IValidator<GetServerByIdQuery> validator,
        IMediator mediator)
    {
        var getServerQuery = new GetServerByIdQuery { Id = id };

        var validationResult = await validator.ValidateAsync(getServerQuery);

        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(validationResult.Errors);
        }

        var result = await mediator.Send(getServerQuery);

        return result.Match(
            onSuccess => TypedResults.Ok(result.Value)
        );
    }

    private static async Task<IResult> Create(
        CreateServerCommand createServerCommand,
        IMediator mediator)
    {
        var result = await mediator.Send(createServerCommand);
        return result.Match(onSuccess => TypedResults.NoContent());
    }

    private static async Task<IResult> Update(
        UpdateServerCommand updateServerCommand,
        IValidator<UpdateServerCommand> validator,
        IMediator mediator)
    {
        var validationResult = await validator.ValidateAsync(updateServerCommand);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(ResponseApiService.Response(StatusCodes.Status400BadRequest, validationResult.Errors));
        }

        var result = await mediator.Send(updateServerCommand);

        if (!result.IsSuccess)
        {
            if (result.Error is ServerErrorBuilder)
            {
                return Results.NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));
            }

            return Results.Json(
                ResponseApiService.Response(StatusCodes.Status500InternalServerError, result.Error),
                statusCode: StatusCodes.Status500InternalServerError);
        }
        return Results.Ok(ResponseApiService.Response(StatusCodes.Status200OK));
    }
}