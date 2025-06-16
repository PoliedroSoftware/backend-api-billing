
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.CreditNote;

[Route("Api/Controllers/v1/CreditNote")]
[ApiController]

public class CreditNoteController(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Summary = "Create new CreditNote")]
    [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(CreateCreditNoteCommand))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
    [HttpPost]
    public async Task<IResult> Create(
            [FromBody] CreateCreditNoteCommand createCreditNoteCommand)
    {
        var result = await mediator.Send(createCreditNoteCommand);
        return result.Match(onSuccess => TypedResults.Ok());
    }

}

