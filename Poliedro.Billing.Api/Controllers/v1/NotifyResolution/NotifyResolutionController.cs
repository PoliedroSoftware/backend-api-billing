using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Helpers;
using Poliedro.Billing.Application.Common.Features;
using Poliedro.Billing.Application.NotifyResolution.Dtos;
using Poliedro.Billing.Application.NotifyResolution.Queries.GetNotifyResolution;
using Poliedro.Billing.Domain.NotifyResolution.Ports;
using Swashbuckle.AspNetCore.Annotations;
namespace Poliedro.Billing.Api.Controllers.v1.NotifyResolution;

[Route("api/v1/notifyresolution")]
[ApiController]
public class NotifyResolutionController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves notify resolution based on the provided API key   
    /// </summary>
    /// <header name="ApiKey"></header>
    /// <returns></returns>
    /// 

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get notify resolution by Bearer token",
        Description = "Retrieves notify resolution based on Bearer token",
        OperationId = "notifyresolution"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successfully Notify Resolution", typeof(NotifyResolutionDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or missing authentication token")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    public async Task<ActionResult<NotifyResolutionDto>> GetNotifyResolution()
    {
        var token = TokenHelper.ExtractBearerToken(Request);

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Authorization header is missing or invalid.");

        var Parameters = new NotifyResolutionParameters(ApiKey: token);

        var notifyResolution = await _mediator.Send(new GetNotifyResolutionByIdQuery(Parameters));

        if (notifyResolution == null)
        {
            var notFoundResponse = ResponseApiService.Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Notify resolution not found."
            );
            return NotFound(notFoundResponse);
        }

        var successResponse = ResponseApiService.Response(
            statusCode: StatusCodes.Status200OK,
            data: notifyResolution,
            message: "Successful query"
        );

        return Ok(successResponse);
    }
}
