using MediatR;
using Microsoft.AspNetCore.Mvc;
using Polideiro.Billing.Application.InvoiceDetailElectronic.Queries.GetAllInvoiceDetailElectronic;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.InvoiceDetailElectronic;

[Route("api/v1/invoicedetail-electronic")]
[ApiController]
public class InvoiceDetailElectronicController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Retrieves invoice detail electronic based on the provided API key
    /// </summary>
    /// <header name="ApiKey"></header>
    /// <param name="pageNumber">Page number for pagination</param>
    /// <param name="pageSize">Items per page for pagination</param>
    /// <param name="orderBy">Order term (required)</param>
    /// <returns></returns>
    /// 

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get invoice detail electronic by Bearer token",
        Description = "Retrieves invoice detail electronic based on Bearer token",
        OperationId = "invoicedetailelectronic"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully Invoice Detail Electronic")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or missing authentication token")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]

    public async Task<ActionResult<InvoiceElectronicDto>> GetAllInvoiceElectronicWithDetails(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string orderBy = "ASC"
        )
    {
        var token = TokenHelper.ExtractBearerToken(Request);

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Authorization header is missing or invalid.");

        var Parameters = new InvoiceElectronicParameters(ApiKey: token, PageNumber: pageNumber, PageSize: pageSize, OrderBy: orderBy);

        var invoiceElectronic = await mediator.Send(new GetAllInvoiceDetailElectronicQuery(Parameters));

        if (invoiceElectronic == null)
        {
            var notFoundResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Invoice detail electronic not found."
            );
            return NotFound(notFoundResponse);
        }

        var successResponse = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            data: invoiceElectronic,
            message: "Successful query"
        );
        return Ok(successResponse);

    }
}
