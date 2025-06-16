namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote
{
    public record TaxTotal(
        int TaxId,
        decimal TaxAmount,
        decimal Percent,
        decimal TaxableAmount
    );
}
