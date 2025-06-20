using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.PrepareInvoicesBilling.Commands.PrepareInvoices;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.PrepareInvoicesBilling;

[ApiController]
[Route("api/v1/prepareinvoicesbilling")]

public class PrepareInvoicesBillingController(IMediator mediator) : ControllerBase
{

    /// <summary>
    /// Prepares invoices for billing.
    /// </summary>
    /// <remarks>
    /// This endpoint prepares invoices for billing by processing the necessary data.
    /// </remarks>
    /// <returns>A response indicating the result of the preparation.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Prepare invoices for billing",
        Description = "This endpoint prepares invoices for billing by processing the necessary data.",
        OperationId = "prepareinvoicesbilling"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Invoices prepared successfully", typeof(object))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access. Please provide a valid API key.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while preparing invoices")]
    [Produces("application/json")]
    
    public async Task<ActionResult<object>> PrepareInvoicesAsync(
        [FromBody] List<object> invoices,
        CancellationToken cancellationToken)
    {
       var token = TokenHelper.ExtractBearerToken(Request);
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Authorization header is missing or invalid.");

        if (invoices == null || !invoices.Any())
        {
            var emptyResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status200OK,
                message: "No invoices."
            );
            return Ok(emptyResponse);
        }

        
        var command = new PrepareInvoicesCommand(invoices, token);
        var result = await mediator.Send(command, cancellationToken);


        var response = ResponseApiService.Response(
                statusCode: StatusCodes.Status200OK,
                message: "Invoices processed successfully.",
                data: invoices
            );

        return Ok(response);

    }
}