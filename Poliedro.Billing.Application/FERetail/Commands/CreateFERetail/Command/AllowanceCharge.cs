namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command
{
    public record AllowanceCharge(
            bool ChargeIndicator,
            string AllowanceChargeReason,
            decimal MultiplierFactorNumeric,
            decimal Amount,
            decimal BaseAmount
        );
}

