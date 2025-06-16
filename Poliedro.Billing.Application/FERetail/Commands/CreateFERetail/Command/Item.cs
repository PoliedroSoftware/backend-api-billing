namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command
{
    public record Item(
       int UnitMeasureId,
       decimal LineExtensionAmount,
       bool FreeOfChargeIndicator,
       List<AllowanceCharge> AllowanceCharges,
       List<TaxTotal> TaxTotals,
       string Description,
       string Notes,
       string Code,
       int TypeItemIdentificationId,
       decimal PriceAmount,
       decimal BaseQuantity,
       decimal InvoicedQuantity
    );
}
