namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command
{
    public record TaxTotal(
            int TaxId,
            decimal TaxAmount,
            decimal Percent,
            decimal TaxableAmount
        );
}
