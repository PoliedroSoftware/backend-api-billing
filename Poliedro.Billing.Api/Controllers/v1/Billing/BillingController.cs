using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Domain.Billing;

using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.Billing;

[Route("api/v1/billing")]
[ApiController]
public class BillingController(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Summary = "Create new Billing")]
    [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.")]
    [Produces("application/json")]
    [HttpPost]
    public async Task<ActionResult<CreateBilling>> CreateBillingCommand(
        [FromBody] IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken)
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


        var command = new CreateBillingCommand(invoices, token);
        var result = await mediator.Send(command, cancellationToken);


        var response = ResponseApiService.Response(
                statusCode: StatusCodes.Status200OK,
                message: "Invoices processed successfully.",
                data: invoices
            );

        return Ok(response);

    }

}






