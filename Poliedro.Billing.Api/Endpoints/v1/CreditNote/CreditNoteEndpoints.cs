using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;
using Poliedro.Billing.Application.CreditNote.Dtos;
using System.ComponentModel.DataAnnotations;


namespace Poliedro.Billing.Api.Endpoints.v1.CreditNote;


public static class CreditNoteEndpoints
{
    public static RouteGroupBuilder MapCreditNoteEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{id}", CreateCreditNoteAsync)
            .WithName("CreateCreditNote")
            .WithTags("CreditNote")
            .WithSummary("Create new Credit Note by Resolution Id")
            .WithDescription("Creates new Credit Note by Resolution Id")
            .Produces<IEnumerable<CreateCreditNoteResultDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);


        return group;
    }


    private static async Task<IResult> CreateCreditNoteAsync(
        int id,
        HttpContext context,
        IMediator mediator,
        [FromBody][Required] IEnumerable<CreateCreditNoteInputDto> creditNotes,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateCreditNoteCommand(id, creditNotes), cancellationToken);
        return TypedResults.Ok(result);
    }
}
