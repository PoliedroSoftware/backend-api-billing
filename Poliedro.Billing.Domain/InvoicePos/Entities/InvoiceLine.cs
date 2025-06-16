namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class InvoiceLine
{
    public int unit_measure_id { get; set; }
    public string invoiced_quantity { get; set; } = default!;
    public string line_extension_amount { get; set; } = default!;
    public bool free_of_charge_indicator { get; set; } = default!;
    public List<TaxTotal> tax_totals { get; set; } = default!;
    public string description { get; set; } = default!;
    public string notes { get; set; } = default!;
    public string code { get; set; } = default!; 
    public int type_item_identification_id { get; set; } = default!;
    public string price_amount { get; set; } = default!;
    public string base_quantity { get; set; } = default!;
    public string _id { get; set; } = default!;
    public List<object> allowance_charges { get; set; } = default!;
}
