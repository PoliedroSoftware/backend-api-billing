using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.CustomersId.Dtos;
using Poliedro.Billing.Application.CustomersId.Queries.CustomersbyId;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.CustomersId
{
    [ApiController]
    [Route("api/v1/customers")]
    public class CustomersIdController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get customer by ID",
            Description = "Returns a customer given their ID and a valid token",
            OperationId = "GetCustomerById"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Customer found", typeof(CustomersDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Customer not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid token")]
        public async Task<IActionResult> GetCustomerById(
            [FromRoute] string id,
            [FromHeader(Name = "Authorization")] string authorization)
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                return Unauthorized("Bearer token is required.");

            string token = authorization["Bearer ".Length..];

            var query = new CustomersIdQuery(id, token);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);

        }
    }
}
