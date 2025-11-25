using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;
using Poliedro.Billing.Application.InvoiceDetailElectronic.Queries.GetAllInvoiceDetailElectronic;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;

namespace Poliedro.Billing.Api.Endpoints.v1.InvoiceDetailElectronic;

public static class InvoiceDetailElectronicEndpoints
{
    public static RouteGroupBuilder MapInvoiceDetailElectronicEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllInvoiceElectronicWithDetails)
            .WithName("GetInvoiceDetailElectronic")
            .WithTags("InvoiceDetailElectronic")
            .WithSummary("Get invoice detail electronic by Bearer token")
            .WithDescription("Retrieves invoice detail electronic based on Bearer token")
            .Produces<InvoiceElectronicDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAllInvoiceElectronicWithDetails(
        HttpContext context,
        int pageNumber = 1,
        int pageSize = 10,
        string orderBy = "ASC",
        IMediator mediator = null!)
    {
        var token = TokenHelper.ExtractBearerToken(context.Request);

        if (string.IsNullOrEmpty(token))
            return Results.Json(
                "Authorization header is missing or invalid.",
                statusCode: StatusCodes.Status401Unauthorized);

        var parameters = new InvoiceElectronicParameters(ApiKey: token, PageNumber: pageNumber, PageSize: pageSize, OrderBy: orderBy);

        var invoiceElectronic = await mediator.Send(new GetAllInvoiceDetailElectronicQuery(parameters));

        if (invoiceElectronic == null)
        {
            var notFoundResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Invoice detail electronic not found."
            );
            return Results.NotFound(notFoundResponse);
        }

        var successResponse = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            data: invoiceElectronic,
            message: "Successful query"
        );
        return Results.Ok(successResponse);
    }
}
