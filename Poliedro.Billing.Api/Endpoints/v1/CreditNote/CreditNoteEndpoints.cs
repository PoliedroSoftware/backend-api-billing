using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;

namespace Poliedro.Billing.Api.Endpoints.v1.CreditNote;

public static class CreditNoteEndpoints
{
    public static RouteGroupBuilder MapCreditNoteEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .WithName("CreateCreditNoteSimple")
            .WithTags("CreditNote")
            .WithSummary("Create new CreditNote")
            .Produces<CreateCreditNoteCommand>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> Create(
        CreateCreditNoteCommand createCreditNoteCommand,
        IMediator mediator)
    {
        var result = await mediator.Send(createCreditNoteCommand);
        return result.Match(onSuccess => TypedResults.Ok());
    }
}
