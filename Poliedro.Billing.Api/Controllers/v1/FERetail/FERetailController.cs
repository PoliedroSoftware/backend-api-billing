
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail;
using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.FERetail;

[Route("Api/Controllers/v1/FERetail")]
[ApiController]

public class FERetailController(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Summary = "Create new FERetail")]
    [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(CreateFERetailCommand))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
    [HttpPost]
    public async Task<IResult> Create(
            [FromBody] CreateFERetailCommand createFERetailCommand)
    {
        var result = await mediator.Send(createFERetailCommand);
        return result.Match(onSuccess => TypedResults.Ok());
    }

}

