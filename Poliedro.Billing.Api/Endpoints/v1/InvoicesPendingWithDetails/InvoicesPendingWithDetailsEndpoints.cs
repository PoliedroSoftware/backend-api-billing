using MediatR;

using Poliedro.Billing.Application.Billing.Dtos;

using Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

namespace Poliedro.Billing.Api.Endpoints.v1.InvoicesPendingWithDetails;

public static class InvoicesPendingWithDetailsEndpoints
{
    public static RouteGroupBuilder MapInvoicesPendingWithDetailsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{id}", GetAllAsync)
            .WithName("GetInvoicesPendingById")
            .WithTags("InvoicesPendingWithDetails")
            .WithSummary("Get pending invoices with details by Id")
            .WithDescription("Retrieves pending invoices with details based on Id")
            .Produces<IEnumerable<CreateBillingDTO>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAllAsync(int Id, IMediator mediator)
    {
        var invoices = await mediator.Send(new InvoicesPendingWithDetailsQuery(Id));
        return TypedResults.Ok(invoices);
    }
}
