namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record ItemElectronicPlemsiDTO
    (
     int unit_measure_id,
     double line_extension_amount,
     bool free_of_charge_indicator,
     List<AllowanceChargePlemsiDTO>? allowance_charges,
     List<TaxTotalPlemsiDTO>? tax_totals,
     List<WIthHoldingTaxTotalPlemsiDTO>? with_holding_tax_total,
     string? description,
     string? notes,
     int? code,
     int type_item_identification_id,
     double price_amount,
     double base_quantity,
     double invoiced_quantity
    );
