using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.Siigo.Commands;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.Siigo
{
    [Route("api/v1/billing/invoices")]
    [ApiController]
    public class SiigoController(IMediator mediator) : ControllerBase
    {
        [SwaggerOperation(Summary = "Invoice basic - Create and Send to DIAN")]
        [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(CreateInvoicesSiigoCommand))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpPost]
        public async Task<IResult> Create(
            [FromBody] CreateInvoicesSiigoCommand command)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Results.Unauthorized();
            }

            var updatedCommand = command with { token = authHeader.ToString().Replace("Bearer ", string.Empty) };

            if (command == null || command.Invoices.Count == 0)
            {
                return Results.BadRequest("The order list is empty or null.");
            }

            var result = await mediator.Send(updatedCommand);

            return result.Match(
                onSuccess => TypedResults.Ok(onSuccess),
                onFailure => TypedResults.BadRequest(onFailure) 
            );
        }
    }
}
