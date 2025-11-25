namespace Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;
public record InvoiceReferenceDTO
{
    public string number { get; init; } //CONSECUTIVO O CBC:ID DE LA FACTURA A REFERENCIAR

    public string uuid { get; init; } //CUFE DE LA FACTURA A REFERENCIAR

    public string issue_date { get; init; } //FECHA DE EMISIÓN DE LA FACTURA A REFERENCIAR
}
