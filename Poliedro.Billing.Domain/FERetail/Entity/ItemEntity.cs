namespace Poliedro.Billing.Domain.FERetail.Entity;

public class ItemEntity
{
    public int unit_measure_id { get; set; }
    public decimal line_extension_amount { get; set; }
    public bool free_of_charge_indicator { get; set; }
    public List<AllowanceChargeEntity>? allowance_charges { get; set; }
    public List<TaxTotalEntity>? tax_totals { get; set; }
    public string? description { get; set; }
    public string? notes { get; set; }
    public string? code { get; set; }
    public int type_item_identification_id { get; set; }
    public decimal price_amount { get; set; }
    public decimal base_quantity { get; set; }
    public decimal invoiced_quantity { get; set; }
}
