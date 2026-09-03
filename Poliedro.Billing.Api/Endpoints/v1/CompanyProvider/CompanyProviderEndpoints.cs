using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Commands.CreateCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Commands.DeleteCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Commands.UpdateCompanyProvider;
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

        group.MapPost("/", Create)
            .WithName("CreateCompanyProvider")
            .WithTags("CompanyProvider")
            .WithSummary("Create company provider billing electronic")
            .Produces<CompanyProviderDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", Update)
            .WithName("UpdateCompanyProvider")
            .WithTags("CompanyProvider")
            .WithSummary("Update company provider billing electronic")
            .Produces<CompanyProviderDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", Delete)
            .WithName("DeleteCompanyProvider")
            .WithTags("CompanyProvider")
            .WithSummary("Delete company provider billing electronic")
            .Produces(StatusCodes.Status204NoContent)
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

    private static async Task<IResult> Create(CreateCompanyProviderDto dto, IMediator mediator)
    {
        var command = new CreateCompanyProviderCommand(dto);
        var result = await mediator.Send(command);
        return Results.Created($"/api/v1/companyProvider/{result.CompanyProviderId}", result);
    }

    private static async Task<IResult> Update(int id, UpdateCompanyProviderDto dto, IMediator mediator)
    {
        var command = new UpdateCompanyProviderCommand(id, dto);
        var result = await mediator.Send(command);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> Delete(int id, IMediator mediator)
    {
        var command = new DeleteCompanyProviderCommand(id);
        var deleted = await mediator.Send(command);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}