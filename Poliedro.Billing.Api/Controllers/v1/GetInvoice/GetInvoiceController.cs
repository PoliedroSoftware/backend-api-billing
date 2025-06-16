using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.GetInvoice.Queries.GetByIdGetInvoice;

namespace Poliedro.Billing.Api.Controllers.v1.GetInvoice
{
    [Route("api/v1/getinvoice")]
    [ApiController]
    public class GetInvoiceController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] string cufe,
            [FromHeader(Name = "Authorization")] string authorization)
        {
            if (string.IsNullOrWhiteSpace(cufe))
            {
                return BadRequest(ResponseApiService.Response(StatusCodes.Status400BadRequest, "CUFE is required"));
            }

            if (string.IsNullOrWhiteSpace(authorization))
            {
                return Unauthorized(ResponseApiService.Response(StatusCodes.Status401Unauthorized, "Authorization header is required"));
            }

            if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(ResponseApiService.Response(StatusCodes.Status401Unauthorized, "Invalid authorization format. Use 'Bearer {token}'"));
            }

            string token = authorization["Bearer ".Length..].Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(ResponseApiService.Response(StatusCodes.Status401Unauthorized, "Token cannot be empty"));
            }

            try
            {

                var data = await mediator.Send(new GetByIdGetInvoiceQuery(cufe, token));

                if (data is null)
                {
                    return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound, "Invoice not found"));
                }

                return Ok(ResponseApiService.Response(StatusCodes.Status200OK, data));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseApiService.Response(StatusCodes.Status500InternalServerError, "An error occurred processing your request"));
            }
        }
    }
}

