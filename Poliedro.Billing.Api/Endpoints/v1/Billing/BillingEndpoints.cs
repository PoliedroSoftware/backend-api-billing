using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Poliedro.Billing.Api.Endpoints.v1.Billing;

public static class BillingEndpoints
{
    public static RouteGroupBuilder MapBillingEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{id}", CreateBillingAsync)
            .WithName("CreateInvoiceElectronic")
            .WithTags("Invoice Electronic")
            .WithSummary("Create new Invoice Electronic By Id")
            .WithDescription("Creates new Invoice Electronic By Id Resolution")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> CreateBillingAsync(int id,
        HttpContext context,
        IMediator mediator,
        [FromBody][Required]CreateBillingRequestDTO request, CancellationToken cancellationToken)
    {
        var invoicesList = await mediator.Send(new CreateBillingCommand(id, request.Data), cancellationToken);
        return TypedResults.Ok(invoicesList);
    }
}
