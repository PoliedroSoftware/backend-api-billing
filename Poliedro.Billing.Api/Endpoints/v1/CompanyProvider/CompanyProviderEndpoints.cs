using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Application.CompanyProvider.Queries.GetCompanyProviderById;

namespace Poliedro.Billing.Api.Endpoints.v1.CompanyProvider;

public static class CompanyProviderEndpoints
{
    public static RouteGroupBuilder MapCompanyProviderEndpoints(this RouteGroupBuilder group)
    {

        group.MapGet("/{id}", GetById)
        .WithName("GetCompanyProviderById")
            .WithTags("CompanyProvider")
            .WithSummary("Get company provider billing electronic")
            .Produces<CompanyProviderDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetById(int id, IMediator mediator)
    {
        var query = new GetCompanyProviderByIdQuery(id);
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }
}
