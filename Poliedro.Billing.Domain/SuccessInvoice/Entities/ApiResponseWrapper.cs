namespace Poliedro.Billing.Domain.SuccessInvoice.Entities;

public record ApiResponseWrapper<T>(
    int Code,
    bool Success,
    string? Info,
    T? Data
);

public record SuccessInvoiceApiResponse(
    List<SuccessInvoice> Docs
);