using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Application.SuccessInvoice.Queries.GetAllSuccessInvoice;
using Poliedro.Billing.Application.SuccessInvoice.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;

namespace Poliedro.Billing.Api.Controllers.v1.SuccessInvoice
{
    [Route("api/v1/invoice")]
    [ApiController]
    public class SuccessInvoiceController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Retrieves all success invoices based on order criteria
        /// </summary>
        /// <param name="authorization">Authentication token (required)</param>
        /// <param name="order">Order term (required)</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="perPage">Items per page for pagination</param>
        /// <returns>List of success invoices</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all success invoices",
            Description = "Retrieves a paginated list of success invoices based on search criteria",
            OperationId = "GetAllSuccessInvoice"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully retrieved invoices", typeof(SuccessInvoiceDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Missing required parameters")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or missing authentication token")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetAllSuccessInvoice(
            [FromHeader(Name = "Authorization")] string authorization,
            [FromQuery] string order = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10
        )
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized("Bearer token is required.");
            }
            string token = authorization["Bearer ".Length..];

            var parameters = new SuccessInvoiceQueryParameters(
                Token: token,
                Order: order,
                Page: page,
                PerPage: perPage
            );

            var successInvoices = await _mediator.Send(new GetAllSuccessInvoiceQuery(parameters));
            return Ok(successInvoices);
        }
    }
}
