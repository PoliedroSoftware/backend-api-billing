using Poliedro.Billing.Domain.Common.Results.Errors;
using System.Net;

namespace Poliedro.Billing.Application.PendingInvoice.Errors;

public class PendingInvoiceErrorBuilder : IError
{
    public const string PEDINGINVOICE_CREATION_ERROR = "PendingInvoiceCreationErrorException";
    public const string PEDINGINVOICE_NOT_FOUND_ERROR = "PendingInvoiceNotFoundErrorException";
    public static Error PendingInvoiceCreationException() => Error.CreateInstance(
        PEDINGINVOICE_CREATION_ERROR,
        "Failed to create PendingInvoice due to an internal error.",
        HttpStatusCode.InternalServerError);

    public const string PEDINGINVOICE_UPDATE_ERROR = "PendingInvoiceUpdateErrorException";

    public static Error PendingInvoiceUpdateException() => Error.CreateInstance(
            PEDINGINVOICE_UPDATE_ERROR,
            "Failed to update PendingInvoice due to an internal error.",
            HttpStatusCode.InternalServerError);
    public static Error PendingInvoiceNotFoundException(int id) => Error.CreateInstance(
       PEDINGINVOICE_NOT_FOUND_ERROR,
       $"PendingInvoice with ID {id} was not found.",
       HttpStatusCode.NotFound);

}

