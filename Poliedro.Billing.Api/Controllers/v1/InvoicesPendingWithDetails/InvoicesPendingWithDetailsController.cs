using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Dtos;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.InvoicesPendingWithDetails;

[ApiController]
[Route("api/v1/invoicespendingwithdetails")]
public class InvoicesPendingWithDetailsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Retrieves pending invoices with details based on the provided API key
    /// </summary>
    /// 
    /// <header name="ApiKey"></header>
    /// 
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get pending invoices with details by Bearer token",
        Description = "Retrieves pending invoices with details based on Bearer token",
        OperationId = "invoicespendingwithdetails"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully retrieved pending invoices with details", typeof(object))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or missing authentication token")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pending invoices with details not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]

    public async Task<ActionResult<object>> GetAllAsync()
    {
        var token = TokenHelper.ExtractBearerToken(Request);

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Authorization header is missing or invalid.");

        var invoicesPendingWithDetails = await mediator.Send(new InvoicesPendingWithDetailsQuery(ApiKey: token));

        if ( !invoicesPendingWithDetails.Any())
        {
            var notFoundResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status200OK,
                message: "Pending invoices not found."
            );
            return Ok(notFoundResponse);

        }
        return Ok(invoicesPendingWithDetails);
    }

}
