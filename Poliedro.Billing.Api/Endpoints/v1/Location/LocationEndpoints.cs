using MediatR;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Dtos;
using Poliedro.Billing.Application.Location.Dtos;
using Poliedro.Billing.Application.Location.Queries.GetAllMunicipalities;

namespace Poliedro.Billing.Api.Endpoints.v1.Location;

public static class LocationEndpoints
{
    public static RouteGroupBuilder MapLocationEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/municipalities", GetAllMunicipalitiesAsync)
            .WithName("GetAllMunicipalities")
            .WithTags("Location")
            .WithSummary("Get all municipalities from Plemsi")
            .WithDescription("Retrieves the complete list of municipalities")
            .Produces<PagedResponseDto<MunicipalityDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAllMunicipalitiesAsync(
        HttpContext context,
        IMediator mediator)
    {
        var token = TokenHelper.ExtractBearerToken(context.Request);
        if (string.IsNullOrEmpty(token))
            return Results.Json(
                "Authorization header is missing or invalid.",
                statusCode: StatusCodes.Status401Unauthorized);

        var result = await mediator.Send(new GetAllMunicipalitiesQuery(ApiKey: token));
        return Results.Ok(result);
    }
}