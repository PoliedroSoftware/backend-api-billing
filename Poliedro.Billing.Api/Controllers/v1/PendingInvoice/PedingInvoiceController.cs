using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.PendingInvoice.Dtos;
using Poliedro.Billing.Application.PendingInvoice.Queries.GellAllPedingInvoice;
using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.PendingInvoice;


[Route("api/v1/pendinginvoice")]
[ApiController]
public class PendingInvoiceController(IMediator mediator) : ControllerBase
{

    /// <summary>
    /// Retrieves pending invoices based on the provided API key
    /// </summary>
    /// 
    /// <header name="ApiKey"></header>
    /// <param name="pageNumber"> Page number for pagination</param> 
    /// <param name="pageSize">Items per page for pagination</param>
    /// <param name="orderBy">Order term (required)</param>
    /// 

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get pending invoices by Bearer token",
        Description = "Retrieves pending invoices based on Bearer token",
        OperationId = "pendinginvoice"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully retrieved pending invoices")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or missing authentication token")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pending invoices not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    

    public async Task<ActionResult<PedingInvoiceDto>> GetAllAsync(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string orderBy = "ASC"
        )
    {
        var token = TokenHelper.ExtractBearerToken(Request);

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Authorization header is missing or invalid.");

        var parameters = new InvoiceElectronicParameters(ApiKey: token, PageNumber: pageNumber, PageSize: pageSize, OrderBy: orderBy);

        var pendingInvoices = await mediator.Send(new GellAllPedingInvoiceQuery(parameters));

        if (pendingInvoices == null)
        {
            var notFoundResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Pending invoices not found."
            );
            return NotFound(notFoundResponse);
        }
        var successResponse = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            data: pendingInvoices,
            message: "Pending invoices retrieved successfully."
        );
        return Ok(successResponse);
    }
}