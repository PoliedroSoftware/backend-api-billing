
namespace Poliedro.Billing.Application.Billing.Services.Factories;

public interface IBillingProviderAdapterFactory
{
    IBillingProviderAdapter Resolve(int providerId);
}

public sealed class BillingProviderAdapterFactory(IEnumerable<IBillingProviderAdapter> adapters)
    : IBillingProviderAdapterFactory
{
    private readonly IReadOnlyDictionary<int, IBillingProviderAdapter> _adapters =
        adapters.ToDictionary(adapter => adapter.ProviderId);

    public IBillingProviderAdapter Resolve(int providerId) =>
        _adapters.TryGetValue(providerId, out var adapter)
            ? adapter
            : throw new InvalidOperationException($"No billing adapter is registered for provider id '{providerId}'.");
}
