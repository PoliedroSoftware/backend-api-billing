using Poliedro.Billing.Domain.Common.Results.Errors;
using System.Net;

namespace Poliedro.Billing.Application.DianResolution.Errors
{
    public class DianResolutionErrorBuilder : IError
    {
        public const string DIAN_RESOLUTION_CREATION_ERROR = "DianResolutionCreationErrorException";
        public const string DIAN_RESOLUTION_UPDATE_ERROR = "DianResolutionUpdateErrorException";
        public const string DIAN_RESOLUTION_NOT_FOUND_ERROR = "DianResolutionNotFoundErrorException";
        public const string NO_DIAN_RESOLUTION_RECORDS_FOUND = "NoDianResolutionRecordsFoundErrorException";
        public const string DIAN_RESOLUTION_DELETION_ERROR = "DianResolutionDeletionErrorException";

        public static Error DianResolutionCreationException() => Error.CreateInstance(
            DIAN_RESOLUTION_CREATION_ERROR,
            "Failed to create Dian Resolution due to an internal error.",
            HttpStatusCode.InternalServerError);
        public static Error DianResolutionUpdateException(int id) => Error.CreateInstance(
            DIAN_RESOLUTION_UPDATE_ERROR,
            $"Failed to update Dian Resolution with ID {id} due to an internal error.",
            HttpStatusCode.InternalServerError);
        public static Error DianResolutionNotFoundException(int id) => Error.CreateInstance(
            DIAN_RESOLUTION_NOT_FOUND_ERROR,
            $"Dian Resolution with ID {id} was not found.",
            HttpStatusCode.NotFound);
        public static Error NoDianResolutionFoundException() => Error.CreateInstance(
            NO_DIAN_RESOLUTION_RECORDS_FOUND,
            "No active Dian Resolution records were found.",
            HttpStatusCode.NotFound);

        public static Error DianResolutionDeletionException(int id) => Error.CreateInstance(
            DIAN_RESOLUTION_DELETION_ERROR,
            $"Failed to delete Dian Resolution with ID {id} due to an internal error.",
            HttpStatusCode.InternalServerError);

    }
}
