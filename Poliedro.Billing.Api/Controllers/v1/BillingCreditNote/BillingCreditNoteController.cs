using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.BillingCreditNote.Commands.CreditNote;
using Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;
using Poliedro.Billing.Application.Common.Features;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
namespace Poliedro.Billing.Api.Controllers.v1.BillingCreditNote;

[Route("api/v1/creditnote")]
[ApiController]
public class BillingCreditNoteController(IMediator _mediator) : ControllerBase
{
    [SwaggerOperation(Summary = "Create new credit note")]
    [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.")]
    [Produces("application/json")]
    [HttpPost]

    public async Task<IActionResult> CreateCreditNoteCommand([FromBody][Required] IEnumerable<CreateCreditNoteInputDTO> invoices, CancellationToken cancellationToken)
    {
        var token = TokenHelper.ExtractBearerToken(Request);
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Authorization header is missing or invalid.");

        if (invoices.IsNullOrEmpty())
        {
            var emptyResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status200OK,
                message: "No invoices.",
                data: invoices
            );
            return Ok(emptyResponse);
        }
        var command = new CreateCreditNoteCommand(invoices, token);

        var result = await _mediator.Send(command, cancellationToken);

        var response = ResponseApiService.Response(
                statusCode: StatusCodes.Status200OK,
                message: "Invoice processing result.",
                data: result
        );
        return Ok(response);
    }
}
