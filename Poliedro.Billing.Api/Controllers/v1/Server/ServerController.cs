using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.Server.Commands.UpdateServer;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Application.Server.Queries.GellAllServer;
using Poliedro.Billing.Application.Server.Queries.GetServerById;
using Poliedro.Billing.Domain.Common.Pagination;
using Swashbuckle.AspNetCore.Annotations;
//using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.Server.Errors;
using Poliedro.Billing.Application.Server.Commands.CreateServer;
using System.ComponentModel.DataAnnotations;
using Poliedro.Billing.Application.Client.Commands.CreateClient;

namespace Poliedro.Billing.Api.Controllers.v1.Server
{
    [Route("api/v1/server")]
    [ApiController]
    public class ServerController(IMediator mediator) : ControllerBase
    {

        /// <summary>
        /// Retrieves all Server records.
        /// </summary>
        /// <returns>A status code indicating the result of the operation and the list of client billing electronic records.</returns>
        /// <response code="200">Returns the list of client billing electronic records.</response>
        /// <response code="404">Returns when there are no client billing electronic records found.</response>
        /// <response code="500">Returns when there is an Internal Server Error.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            var data = await mediator.Send(new GellAllServerQuery(new PaginationParams { PageNumber = paginationParams.PageNumber, PageSize = paginationParams.PageSize }));
            if (data is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseApiService.Response(StatusCodes.Status404NotFound));
            }


            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }
        [SwaggerOperation(Summary = "Get Server")]
        [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(ServerDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified server does not exist.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpGet("{id}")]
        public async Task<IResult> GetById([FromRoute] int id, [FromServices] IValidator<GetServerByIdQuery> validator)
        {
            var getServerQuery = new GetServerByIdQuery { Id = id };

            var validationResult = await validator.ValidateAsync(getServerQuery);

            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(validationResult.Errors);
            }

            var result = await mediator.Send(getServerQuery);

            return result.Match(
                onSuccess => TypedResults.Ok(result.Value)
            );
        }

        [SwaggerOperation(
            Summary = "Create new Server")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpPost]

        public async Task<IResult> Create(
            [FromBody] CreateServerCommand createServerCommand)
        {
            var result = await mediator.Send(createServerCommand);
            return result.Match(onSuccess => TypedResults.NoContent());
        }

        [SwaggerOperation(Summary = "Update an existing Server")]
        [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The requested server was not found.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]

        [HttpPut]
        public async Task<IActionResult> Update(
        [FromBody] UpdateServerCommand updateServerCommand,
        [FromServices] IValidator<UpdateServerCommand> validator)
        {
            var validationResult = await validator.ValidateAsync(updateServerCommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(ResponseApiService.Response(StatusCodes.Status400BadRequest, validationResult.Errors));
            }

            var result = await mediator.Send(updateServerCommand);

            if (!result.IsSuccess)
            {
                if (result.Error is ServerErrorBuilder)
                {
                    return NotFound(ResponseApiService.Response(StatusCodes.Status404NotFound));
                }

                return StatusCode(StatusCodes.Status500InternalServerError, ResponseApiService.Response(StatusCodes.Status500InternalServerError, result.Error));
            }
            return Ok(ResponseApiService.Response(StatusCodes.Status200OK));
        }
    }
}
