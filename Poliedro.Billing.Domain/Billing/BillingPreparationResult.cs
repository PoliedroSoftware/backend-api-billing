namespace Poliedro.Billing.Domain.Billing;

using Poliedro.Billing.Domain.Billing.Pos.Entity;
using Poliedro.Billing.Domain.FERetail.Entity;

/// <summary>
/// Resultado de preparar una factura para el proveedor. Cuando <see cref="Error"/>
/// tiene valor, la factura no debe enviarse y el motivo se reporta al cliente.
/// </summary>
public sealed record BillingPreparationResult(CreateBilling Billing, PreparedBillingDocument? Output, string? Error);

/// <summary>Provider-neutral prepared document, tagged by its billing document family.</summary>
public sealed record PreparedBillingDocument
{
    public FERetailelectronicEntity? ElectronicInvoice { get; }
    public InvoicePosEntity? PointOfSaleInvoice { get; }

    private PreparedBillingDocument(FERetailelectronicEntity? electronicInvoice, InvoicePosEntity? pointOfSaleInvoice)
    {
        if ((electronicInvoice is null) == (pointOfSaleInvoice is null))
            throw new ArgumentException("Exactly one prepared billing document must be supplied.");

        ElectronicInvoice = electronicInvoice;
        PointOfSaleInvoice = pointOfSaleInvoice;
    }

    public static PreparedBillingDocument FromElectronicInvoice(FERetailelectronicEntity invoice) =>
        new(invoice ?? throw new ArgumentNullException(nameof(invoice)), null);

    public static PreparedBillingDocument FromPointOfSaleInvoice(InvoicePosEntity invoice) =>
        new(null, invoice ?? throw new ArgumentNullException(nameof(invoice)));
}
