using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.BillingCreditNote.Commands.CreditNote;
using Poliedro.Billing.Application.Common.Features;

namespace Poliedro.Billing.Api.Endpoints.v1.BillingCreditNote;

public static class BillingCreditNoteEndpoints
{
    public static RouteGroupBuilder MapBillingCreditNoteEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCreditNote)
            .WithName("CreateCreditNote")
            .WithTags("BillingCreditNote")
            .WithSummary("Create new credit note")
            .WithDescription("Creates new credit note records")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> CreateCreditNote(
        HttpContext context,
        IEnumerable<CreateBillingInputDTO> invoices,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var token = TokenHelper.ExtractBearerToken(context.Request);
        if (string.IsNullOrEmpty(token))
            return Results.Unauthorized();

        if (invoices.IsNullOrEmpty())
        {
            var emptyResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status200OK,
                message: "No invoices.",
                data: invoices
            );
            return Results.Ok(emptyResponse);
        }

        var command = new CreateCreditNoteCommand(invoices, token);
        var result = await mediator.Send(command, cancellationToken);

        var response = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            message: "Invoice processing result.",
            data: result
        );
        return Results.Ok(response);
    }
}
