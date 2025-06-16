using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.Client.Commands.CreateClient;
using Poliedro.Billing.Application.Client.Commands.DeleteClient;
using Poliedro.Billing.Application.Client.Commands.UpdateClient;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Application.Client.Queries.GetAllClient;
using Poliedro.Billing.Application.Client.Queries.GetClientBillingElectronicById;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.Client
{
    /// <summary>
    /// Controller for handling client billing electronic operations.
    /// </summary>
    ///
    [Route("api/v1/client")]
    [ApiController]
    public class ClientController(IMediator mediator) : ControllerBase
    {

        [SwaggerOperation(
          Summary = "Create new client billing electronic")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]

        [HttpPost]
        public async Task<IResult> Create(
            [FromBody] CreateClientCommand createClientBillingElectronicCommand)
        {
            var result = await mediator.Send(createClientBillingElectronicCommand);
            return result.Match(onSuccess => TypedResults.NoContent());
        }

        [SwaggerOperation(Summary = "Update client billing electronic")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified client billing electronic does not exist.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpPut]
        public async Task<IResult> Update(
            [FromBody] UpdateClientCommand updateClientBillingElectronicCommand)
        {
            var result = await mediator.Send(updateClientBillingElectronicCommand);
            return result.Match(onSuccess => TypedResults.NoContent());

        }

        [SwaggerOperation(Summary = "Get all clients billing electronic")]
        [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(IEnumerable<ClientDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No found clients billing electronic.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpGet]
        public async Task<IResult> GetAll()
        {
            var result = await mediator.Send(new GetAllClientQuery());
            return result.Match(onSuccess => TypedResults.Ok(result.Value));
        }

        [SwaggerOperation(Summary = "Get client billing electronic")]
        [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(ClientDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified client billing electronic does not exist.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpGet("{id}")]
        public async Task<IResult> GetById(int id)
        {
            var getClientBillintElectronicQuery = new GetClientByIdQuery { Id = id };
            var result = await mediator.Send(getClientBillintElectronicQuery);
            return result.Match(onSuccess => TypedResults.Ok(result.Value));
        }

        [SwaggerOperation(Summary = "Delete client billing electronic")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The operation was successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified client billing electronic does not exist.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
        [HttpDelete("{id}")]
        public async Task<IResult> Delete(int id)
        {
            var deleteClientBillintElectronicCommand = new DeleteClientCommand { Id = id };
            var result = await mediator.Send(deleteClientBillintElectronicCommand);
            return result.Match(onSuccess => TypedResults.NoContent());
        }
    }
}
