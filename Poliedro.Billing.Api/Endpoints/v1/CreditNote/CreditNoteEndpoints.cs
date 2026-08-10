using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;
using Poliedro.Billing.Domain.CreditNote.Entity;
using System.ComponentModel.DataAnnotations;

namespace Poliedro.Billing.Api.Endpoints.v1.CreditNote;

public static class CreditNoteEndpoints
{
    public static RouteGroupBuilder MapCreditNoteEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCreditNoteAsync)
            .WithName("CreateCreditNote")
            .WithTags("CreditNote")
            .WithSummary("Create new Credit Note")
            .Produces<ApiResponseCreditNote>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> CreateCreditNoteAsync(
        IMediator mediator,
        [FromBody][Required] CreateCreditNoteCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return TypedResults.Problem(result.Error!.Description);

        return TypedResults.Ok(result.Value);
    }
}
