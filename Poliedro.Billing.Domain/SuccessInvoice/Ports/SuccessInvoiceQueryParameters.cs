namespace Poliedro.Billing.Domain.SuccessInvoice.Ports;

public record SuccessInvoiceQueryParameters(
     string Token,
     string Order,
     int Page,
     int PerPage
     );
