using Poliedro.Billing.Domain.Common.Results.Errors;
using System.Net;

namespace Poliedro.Billing.Application.GetInvoice.Errors;

public class GetInvoiceErrorBuilder : IError
{
    public const string GETINVOICE_CREATION_ERROR = "GetInvoiceCreationErrorException";
    public const string GETINVOICE_NOT_FOUND_ERROR = "GetInvoiceNotFoundErrorException";
    public static Error GetInvoiceCreationException() => Error.CreateInstance(
        GETINVOICE_CREATION_ERROR,
        "Failed to create GetInvoice due to an internal error.",
        HttpStatusCode.InternalServerError);

    public const string GETINVOICE_UPDATE_ERROR = "GetInvoiceUpdateErrorException";

    public static Error GetInvoiceUpdateException() => Error.CreateInstance(
            GETINVOICE_UPDATE_ERROR,
            "Failed to update GetInvoice due to an internal error.",
            HttpStatusCode.InternalServerError);
    public static Error GetInvoiceNotFoundException(int id) => Error.CreateInstance(
       GETINVOICE_NOT_FOUND_ERROR,
       $"GetInvoice with ID {id} was not found.",
       HttpStatusCode.NotFound);
}
