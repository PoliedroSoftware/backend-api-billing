namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record ItemElectronicRequestFEDTO
{
     public int unit_measure_id {  get; init; }
     public double line_extension_amount { get; init; }

    public double total_before_tax { get; init; }
    public double discount_amount_by_invoice { get; init; }
    public string? lidiscount_type { get; init; }


    public double unit_price_before_discount { get; init; }
     public double line_discount_amount { get; init; }
     public string? line_discount_type { get; init; }

     public bool free_of_charge_indicator { get; init; }
     public List<AllowanceChargeRequestFEDTO>? allowance_charges { get; init; }
     public List<TaxTotalRequestFEDTO>? tax_totals { get; init; }
     public List<WIthHoldingTaxTotalRequestFEDTO>? with_holding_tax_total { get; init; }
     public string? description { get; init; }
     public string? notes { get; init; }
     public string? code { get; init; }
     public int type_item_identification_id { get; init; }
     public double price_amount { get; init; }
     public double base_quantity { get; init; }
     public double invoiced_quantity { get; init; }
     
}
