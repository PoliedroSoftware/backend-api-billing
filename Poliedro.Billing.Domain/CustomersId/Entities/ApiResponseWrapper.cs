

namespace Poliedro.Billing.Domain.CustomersId.Entities;

    public record ApiResponseWrapper<T>(
    int Code,
    bool Success,
    string? Info,
    T? Data
);
