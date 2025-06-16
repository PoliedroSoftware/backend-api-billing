namespace Poliedro.Billing.Application.InvoicePendingWithDetails.Dtos;

public record DetailsInvoiceFEPendingDto
    (
    int Id,
    int Transaccion,
    int Code,
    int Type_item_identification_id,
    string Description,
    int Unit_measure_id,
    double Base_quantity,
    double Invoiced_quantity,
    double Price_amount,
    double Line_extension_amount,
    double Tax_amount,
    double Percent,
    double Unit_preci
    );