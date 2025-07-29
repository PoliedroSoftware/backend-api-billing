namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record ItemElectronicRequestDTO
{
     public int unit_measure_id {  get; init; }
     public double line_extension_amount { get; init; }
     public bool free_of_charge_indicator { get; init; }
     public List<AllowanceChargeRequestDTO>? allowance_charges { get; init; }
     public List<TaxTotalRequestDTO>? tax_totals { get; init; }
     public List<WIthHoldingTaxTotalRequestDTO>? with_holding_tax_total { get; init; }
     public string? description { get; init; }
     public string? notes { get; init; }
     public int? code { get; init; }
     public int type_item_identification_id { get; init; }
     public double price_amount { get; init; }
     public double base_quantity { get; init; }
     public double invoiced_quantity { get; init; }
    }
