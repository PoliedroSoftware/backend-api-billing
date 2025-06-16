using Poliedro.Billing.Domain.Common.Results.Errors;
using System.Net;

namespace Poliedro.Billing.Application.Server.Errors;

public class ServerErrorBuilder : IError
{
    public const string SERVER_CREATION_ERROR = "ServerCreationErrorException";
    public const string SERVER_NOT_FOUND_ERROR = "ServerNotFoundErrorException";
    public static Error ServerCreationException() => Error.CreateInstance(
        SERVER_CREATION_ERROR,
        "Failed to create Server due to an internal error.",
        HttpStatusCode.InternalServerError);

    public const string SERVER_UPDATE_ERROR = "ServerUpdateErrorException";

    public static Error ServerUpdateException() => Error.CreateInstance(
            SERVER_UPDATE_ERROR,
            "Failed to update Server due to an internal error.",
            HttpStatusCode.InternalServerError);
    public static Error ServerNotFoundException(int id) => Error.CreateInstance(
       SERVER_NOT_FOUND_ERROR,
       $"Server with ID {id} was not found.",
       HttpStatusCode.NotFound);

}

