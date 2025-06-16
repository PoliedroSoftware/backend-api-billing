namespace Poliedro.Billing.Domain.InvoicesPendingWithDetails.Entities;
public class DetailsInvoiceFEPendingEntity
{
    public int Id { get; set; } = default!;
    public int Transaccion { get; set; } = default!;
    public int Code { get; set; } = default!;
    public int Type_item_identification_id { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Unit_measure_id { get; set; } = default!;
    public double Base_quantity { get; set; } = default!;
    public double Invoiced_quantity { get; set; } = default!;
    public double Price_amount { get; set; } = default!;
    public double Line_extension_amount { get; set; } = default!;
    public double Tax_amount { get; set; } = default!;
    public double Percent { get; set; } = default!;
    public double Unit_preci { get; set; } = default!;
}
