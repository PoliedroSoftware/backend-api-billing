using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.DeleteDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.UpdateDianResolution;
using Poliedro.Billing.Application.DianResolution.Dtos;
using Poliedro.Billing.Application.DianResolution.Queries.GetAllDianResolution;
using Poliedro.Billing.Application.DianResolution.Queries.GetDianResolutionById;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.DianResolution
{
    /// <summary>
    /// Controller for handling DIAN resolution operations.
    /// </summary>
    [Route("api/v1/dianresolution")]
    [ApiController]
    public class DianResolutionController(IMediator mediator) : ControllerBase
    {
        [SwaggerOperation(
          Summary = "Create new Dian Resolution")]
        [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpPost]

        public async Task<IResult> Create(
            [FromBody] CreateDianResolutionCommand createDianResolutionCommand)
        {
            var result = await mediator.Send(createDianResolutionCommand);
            return result.Match(onSuccess => TypedResults.NoContent());
        }

        [SwaggerOperation(Summary = "Get all Dian Resolution")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No found Dian Resolution.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpGet]
        public async Task<IResult> GetAll()
        {
            var result = await mediator.Send(new GetAllDianResolutionQuery());
            return result.Match(onSuccess => TypedResults.Ok(result.Value));
        }

        [SwaggerOperation(Summary = "Get Dian Resolution")]
        [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(DianResolutionDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified Dian Resolution does not exist.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpGet("{id}")]
        public async Task<IResult> GetById(int id)
        {
            var getDianResolutionQuery = new GetDianResolutionByIdQuery { Id = id };
            var result = await mediator.Send(getDianResolutionQuery);
            return result.Match(onSuccess => TypedResults.Ok(result.Value));
        }

        [SwaggerOperation(Summary = "Update dian resolution")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified dian resolution does not exist.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpPatch]

        public async Task<IResult> Update(
            [FromBody] UpdateDianResolutionCommand updateDianResolutionCommand)
        {
            var result = await mediator.Send(updateDianResolutionCommand);
            return result.Match(onSuccess => TypedResults.NoContent());

        }

        [SwaggerOperation(Summary = "Delete Dian Resolution")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified Dian Resolution does not exist.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpDelete("{id}")]
        public async Task<IResult> Delete(int id)
        {
            var deleteDianResolutionCommand = new DeleteDianResolutionCommand { Id = id };
            var result = await mediator.Send(deleteDianResolutionCommand);
            return result.Match(onSuccess => TypedResults.NoContent());
        }
    }
}

