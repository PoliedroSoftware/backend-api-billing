namespace Poliedro.Billing.Domain.FERetail.Entity;

public class ItemsInvoiceEntity
{
    public int id { get; set; } = default!;
    public int transaccion { get; set; } = default!;
    public int code { get; set; } = default!;
    public int type_item_identification_id { get; set; } = default!;
    public string description { get; set; } = default!;
    public int unit_measure_id { get; set; } = default!;
    public double base_quantity { get; set; } = default!;
    public double invoiced_quantity { get; set; } = default!;
    public double price_amount { get; set; } = default!;
    public double line_extension_amount { get; set; } = default!;
    public double tax_amount { get; set; } = default!;
    public double percent { get; set; } = default!;
    public double unit_preci { get; set; } = default!;
}
