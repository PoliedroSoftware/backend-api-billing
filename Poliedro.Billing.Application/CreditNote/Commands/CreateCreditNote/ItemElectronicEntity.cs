namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote
{
    public record ItemElectronicEntity(
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
