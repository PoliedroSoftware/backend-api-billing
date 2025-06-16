using Poliedro.Billing.Application.Common.Constants;
using Poliedro.Billing.Domain.Common.Results.Errors;
using System.Net;

namespace Poliedro.Billing.Application.Client.Errors;

public class ClientBillingElectronicErrorBuilder : IError
{
    public const string CLIENT_BILLING_CREATION_ERROR = "ClientBillingCreationErrorException";
    public const string CLIENT_BILLING_NOT_FOUND_ERROR = "ClientBillingNotFoundErrorException";
    public const string CLIENT_BILLING_UPDATE_ERROR = "ClientBillingUpdateErrorException";
    public const string NO_CLIENT_BILLING_RECORDS_FOUND = "NoClientBillingRecordsFoundErrorException";
    public const string CLIENT_BILLING_DELETION_ERROR = "ClientBillingDeletionErrorException";
    public static Error ClientBillingCreationException() => Error.CreateInstance(
        CLIENT_BILLING_CREATION_ERROR,
        "Failed to create Client Billing Electronic due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error ClientBillingNotFoundException(int id) => Error.CreateInstance(
        CLIENT_BILLING_NOT_FOUND_ERROR,
        $"Client Billing Electronic with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error ClientBillingUpdateException(int id) => Error.CreateInstance(
        CLIENT_BILLING_UPDATE_ERROR,
        $"Failed to update Client Billing Electronic with ID {id} due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error NoClientBillingRecordsFoundException() => Error.CreateInstance(
        NO_CLIENT_BILLING_RECORDS_FOUND,
        "No active Client Billing Electronic records were found.",
        HttpStatusCode.NotFound);

    public static Error ClientBillingDeletionException(int id) => Error.CreateInstance(
        CLIENT_BILLING_DELETION_ERROR,
        $"Failed to delete Client Billing Electronic with ID {id} due to an internal error.",
        HttpStatusCode.InternalServerError);
}
