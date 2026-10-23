using Poliedro.Billing.Application.Client.Errors;
using Poliedro.Billing.Application.CreditNote.Errors;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingErrorContractTests
{
    [Fact]
    public void Client_error_factory_preserves_codes_messages_and_http_semantics()
    {
        AssertError(ClientBillingElectronicErrorBuilder.ClientBillingCreationException(), ClientBillingElectronicErrorBuilder.CLIENT_BILLING_CREATION_ERROR, System.Net.HttpStatusCode.InternalServerError);
        AssertError(ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(42), ClientBillingElectronicErrorBuilder.CLIENT_BILLING_NOT_FOUND_ERROR, System.Net.HttpStatusCode.NotFound);
        Assert.Contains("42", ClientBillingElectronicErrorBuilder.ClientBillingNotFoundException(42).Description);
        AssertError(ClientBillingElectronicErrorBuilder.ClientBillingUpdateException(42), ClientBillingElectronicErrorBuilder.CLIENT_BILLING_UPDATE_ERROR, System.Net.HttpStatusCode.InternalServerError);
        AssertError(ClientBillingElectronicErrorBuilder.NoClientBillingRecordsFoundException(), ClientBillingElectronicErrorBuilder.NO_CLIENT_BILLING_RECORDS_FOUND, System.Net.HttpStatusCode.NotFound);
        AssertError(ClientBillingElectronicErrorBuilder.ClientBillingDeletionException(42), ClientBillingElectronicErrorBuilder.CLIENT_BILLING_DELETION_ERROR, System.Net.HttpStatusCode.InternalServerError);
        AssertError(ClientBillingElectronicErrorBuilder.ClientBillingNotFoundByApiKeyException(), ClientBillingElectronicErrorBuilder.CLIENT_BILLING_NOT_FOUND_BY_API_KEY_ERROR, System.Net.HttpStatusCode.NotFound);
        AssertError(ClientBillingElectronicErrorBuilder.InvalidResolutionTypeException(), ClientBillingElectronicErrorBuilder.INVALID_RESOLUTION_TYPE_ERROR, System.Net.HttpStatusCode.BadRequest);
        Assert.Contains("storage unavailable", ClientBillingElectronicErrorBuilder.GetLastInvoiceNumberException("storage unavailable").Description);
    }

    [Fact]
    public void Credit_note_error_factory_returns_expected_client_error_contract()
    {
        AssertError(CreditNoteBillingElectronicErrorBuilder.CreditNoteCreationException(), CreditNoteBillingElectronicErrorBuilder.CREDIT_NOTE_CREATION_ERROR, System.Net.HttpStatusCode.InternalServerError);
        AssertError(CreditNoteBillingElectronicErrorBuilder.CreditNoteNotFoundException(9), CreditNoteBillingElectronicErrorBuilder.CREDIT_NOTE_NOT_FOUND_ERROR, System.Net.HttpStatusCode.NotFound);
        Assert.Contains("9", CreditNoteBillingElectronicErrorBuilder.CreditNoteNotFoundException(9).Description);
        AssertError(CreditNoteBillingElectronicErrorBuilder.CreditNoteUpdateException(9), CreditNoteBillingElectronicErrorBuilder.CREDIT_NOTE_UPDATE_ERROR, System.Net.HttpStatusCode.InternalServerError);
        AssertError(CreditNoteBillingElectronicErrorBuilder.NoCreditNoteRecordsFoundException(), CreditNoteBillingElectronicErrorBuilder.NO_CREDIT_NOTE_RECORDS_FOUND, System.Net.HttpStatusCode.NotFound);
        AssertError(CreditNoteBillingElectronicErrorBuilder.CreditNoteDeletionException(9), CreditNoteBillingElectronicErrorBuilder.CREDIT_NOTE_DELETION_ERROR, System.Net.HttpStatusCode.InternalServerError);
    }

    private static void AssertError(Error error, string code, System.Net.HttpStatusCode status)
    {
        Assert.Equal(code, error.Code);
        Assert.Equal(status, error.HttpStatusCode);
        Assert.False(string.IsNullOrWhiteSpace(error.Description));
    }
}
