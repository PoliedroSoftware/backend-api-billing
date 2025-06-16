using Poliedro.Billing.Application.Common.Constants;
using Poliedro.Billing.Domain.Common.Results.Errors;
using System.Net;

namespace Poliedro.Billing.Application.CreditNote.Errors;

public class CreditNoteBillingElectronicErrorBuilder : IError
{
    public const string CREDIT_NOTE_CREATION_ERROR = "CreditNoteCreationErrorException";
    public const string CREDIT_NOTE_NOT_FOUND_ERROR = "CreditNoteNotFoundErrorException";
    public const string CREDIT_NOTE_UPDATE_ERROR = "CreditNoteUpdateErrorException";
    public const string NO_CREDIT_NOTE_RECORDS_FOUND = "NoCreditNoteRecordsFoundErrorException";
    public const string CREDIT_NOTE_DELETION_ERROR = "CreditNoteDeletionErrorException";

    public static Error CreditNoteCreationException() => Error.CreateInstance(
        CREDIT_NOTE_CREATION_ERROR,
        "Failed to create Credit Note due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error CreditNoteNotFoundException(int id) => Error.CreateInstance(
        CREDIT_NOTE_NOT_FOUND_ERROR,
        $"Credit Note with ID {id} was not found.",
        HttpStatusCode.NotFound);

    public static Error CreditNoteUpdateException(int id) => Error.CreateInstance(
        CREDIT_NOTE_UPDATE_ERROR,
        $"Failed to update Credit Note with ID {id} due to an internal error.",
        HttpStatusCode.InternalServerError);

    public static Error NoCreditNoteRecordsFoundException() => Error.CreateInstance(
        NO_CREDIT_NOTE_RECORDS_FOUND,
        "No active Credit Note records were found.",
        HttpStatusCode.NotFound);

    public static Error CreditNoteDeletionException(int id) => Error.CreateInstance(
        CREDIT_NOTE_DELETION_ERROR,
        $"Failed to delete Credit Note with ID {id} due to an internal error.",
        HttpStatusCode.InternalServerError);
}
