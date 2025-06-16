namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote
{
    public record AllowanceCharge(
        bool ChargeIndicator,
        string AllowanceChargeReason,
        decimal MultiplierFactorNumeric,
        decimal Amount,
        decimal BaseAmount
    );
}
