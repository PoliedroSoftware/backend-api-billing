namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote
{
    public record InvoiceReference(
        string IssueDate,
        string Uuid,
        string Number
    );
}
