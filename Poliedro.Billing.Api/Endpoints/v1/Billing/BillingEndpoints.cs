using MediatR;
using Microsoft.IdentityModel.Tokens;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Common.Features;

namespace Poliedro.Billing.Api.Endpoints.v1.Billing;

public static class BillingEndpoints
{
    public static RouteGroupBuilder MapBillingEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateBilling)
            .WithName("CreateBilling")
            .WithTags("Billing")
            .WithSummary("Create new Billing")
            .WithDescription("Creates new billing records")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> CreateBilling(
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

        var command = new CreateBillingCommand(invoices, token);
        var result = await mediator.Send(command, cancellationToken);

        var response = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            message: "Invoice processing result.",
            data: result
        );
        return Results.Ok(response);
    }
}
