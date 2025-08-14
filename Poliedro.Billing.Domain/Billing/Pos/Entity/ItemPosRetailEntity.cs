

namespace Poliedro.Billing.Domain.Billing.Pos.Entity;
public class ItemPosRetailEntity
{
   
    public int unit_measure_id { get; set; }
    public string invoiced_quantity { get; set; }
    public string line_extension_amount { get; set; }
    public bool free_of_charge_indicator { get; set; }
    public List<TaxTotalPosEntity> tax_totals { get; set; }
    public string description { get; set; }
    public string notes { get; set; }
    public string code { get; set; }
    public int type_item_identification_id { get; set; }
    public string price_amount { get; set; }
    public string base_quantity { get; set; }
}

