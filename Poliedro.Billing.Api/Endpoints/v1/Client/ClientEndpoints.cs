using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.Client.Commands.CreateClient;
using Poliedro.Billing.Application.Client.Commands.DeleteClient;
using Poliedro.Billing.Application.Client.Commands.UpdateClient;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Application.Client.Queries.GetAllClient;
using Poliedro.Billing.Application.Client.Queries.GetClientBillingElectronicById;

namespace Poliedro.Billing.Api.Endpoints.v1.Client;

public static class ClientEndpoints
{
    public static RouteGroupBuilder MapClientEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .WithName("CreateClient")
            .WithTags("Client")
            .WithSummary("Create new client billing electronic")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/", Update)
            .WithName("UpdateClient")
            .WithTags("Client")
            .WithSummary("Update client billing electronic")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetAll)
            .WithName("GetAllClients")
            .WithTags("Client")
            .WithSummary("Get all clients billing electronic")
            .Produces<IEnumerable<ClientDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetById)
            .WithName("GetClientById")
            .WithTags("Client")
            .WithSummary("Get client billing electronic")
            .Produces<ClientDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", Delete)
            .WithName("DeleteClient")
            .WithTags("Client")
            .WithSummary("Delete client billing electronic")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> Create(
        CreateClientCommand createClientBillingElectronicCommand,
        IMediator mediator)
    {
        var result = await mediator.Send(createClientBillingElectronicCommand);
        return result.Match(onSuccess => TypedResults.NoContent());
    }

    private static async Task<IResult> Update(
        UpdateClientCommand updateClientBillingElectronicCommand,
        IMediator mediator)
    {
        var result = await mediator.Send(updateClientBillingElectronicCommand);
        return result.Match(onSuccess => TypedResults.NoContent());
    }

    private static async Task<IResult> GetAll(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllClientQuery());
        return result.Match(onSuccess => TypedResults.Ok(result.Value));
    }

    private static async Task<IResult> GetById(int id, IMediator mediator)
    {
        var getClientBillintElectronicQuery = new GetClientByIdQuery { Id = id };
        var result = await mediator.Send(getClientBillintElectronicQuery);
        return result.Match(onSuccess => TypedResults.Ok(result.Value));
    }

    private static async Task<IResult> Delete(int id, IMediator mediator)
    {
        var deleteClientBillintElectronicCommand = new DeleteClientCommand { Id = id };
        var result = await mediator.Send(deleteClientBillintElectronicCommand);
        return result.Match(onSuccess => TypedResults.NoContent());
    }
}
