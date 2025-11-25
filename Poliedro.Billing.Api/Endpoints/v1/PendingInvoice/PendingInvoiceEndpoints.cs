using MediatR;
using Microsoft.AspNetCore.Http;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.PendingInvoice.Dtos;
using Poliedro.Billing.Application.PendingInvoice.Queries.GellAllPedingInvoice;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;

namespace Poliedro.Billing.Api.Endpoints.v1.PendingInvoice;

public static class PendingInvoiceEndpoints
{
    public static RouteGroupBuilder MapPendingInvoiceEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllAsync)
            .WithName("GetPendingInvoices")
            .WithTags("PendingInvoice")
            .WithSummary("Get pending invoices by Bearer token")
            .WithDescription("Retrieves pending invoices based on Bearer token")
            .Produces<PedingInvoiceDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }

    private static async Task<IResult> GetAllAsync(
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

        var pendingInvoices = await mediator.Send(new GellAllPedingInvoiceQuery(parameters));

        if (pendingInvoices == null)
        {
            var notFoundResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Pending invoices not found."
            );
            return Results.NotFound(notFoundResponse);
        }
        var successResponse = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            data: pendingInvoices,
            message: "Pending invoices retrieved successfully."
        );
        return Results.Ok(successResponse);
    }
}
