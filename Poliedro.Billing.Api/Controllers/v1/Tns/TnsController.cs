using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.Tns.Commands;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.Tns
{
    [Route("api/v1/billing/sales/create")]
    [ApiController]
    public class TnsController(IMediator mediator) : ControllerBase
    {
        [SwaggerOperation(Summary = "Create new Billings TNS")]
        [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(CreateSalesTNSCommand))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpPost]
        public async Task<IResult> Create(
            [FromBody] CreateSalesTNSCommand command)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Results.Unauthorized();
            }

            var updatedCommand = command with { token = authHeader.ToString().Replace("Bearer ", string.Empty) };

            if (command == null || command.Orders.Count == 0)
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
