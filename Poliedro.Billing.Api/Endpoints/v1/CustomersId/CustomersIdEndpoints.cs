using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Application.CustomersId.Dtos;
using Poliedro.Billing.Application.CustomersId.Queries.CustomersbyId;

namespace Poliedro.Billing.Api.Endpoints.v1.CustomersId;

public static class CustomersIdEndpoints
{
    public static RouteGroupBuilder MapCustomersIdEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", GetCustomerById)
            .WithName("GetCustomerById")
            .WithTags("Customers")
            .WithSummary("Get customer by ID")
            .WithDescription("Returns a customer given their ID and a valid token")
            .Produces<CustomersDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return group;
    }

    private static async Task<IResult> GetCustomerById(
        HttpContext context,
        string id,
        IMediator mediator)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authorization) || 
            string.IsNullOrEmpty(authorization) || 
            !authorization.ToString().StartsWith("Bearer "))
        {
            return Results.Unauthorized();
        }

        string token = authorization.ToString()["Bearer ".Length..];

        var query = new CustomersIdQuery(id, token);
        var result = await mediator.Send(query);

        if (result == null)
            return Results.NotFound();

        return Results.Ok(result);
    }
}
