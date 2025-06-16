namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command
{
    public record GeneralAllowance(
            string AllowanceChargeReason,
            decimal AllowancePercent,
            decimal Amount,
            decimal BaseAmount
    );
}
