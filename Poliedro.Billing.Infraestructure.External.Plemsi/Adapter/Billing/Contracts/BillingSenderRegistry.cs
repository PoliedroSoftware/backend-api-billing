using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;

/// <summary>
/// Resolves the infrastructure adapter registered for a provider and document type.
/// </summary>
public class BillingSenderRegistry : IBillingSenderFactory
{
    private readonly IDictionary<(int ProviderId, ResolutionType ResolutionType), IBillingSender> _senders;

    public BillingSenderRegistry(IEnumerable<IBillingSender> senders)
    {
        var registrations = new Dictionary<(int, ResolutionType), IBillingSender>();
        foreach (var sender in senders)
        {
            var adapterType = GetUnderlyingSenderType(sender);
            var registration = adapterType
                .GetCustomAttributes(typeof(BillingSenderAdapterAttribute), inherit: false)
                .OfType<BillingSenderAdapterAttribute>()
                .SingleOrDefault()
                ?? throw new Exception($"Sender type {adapterType.FullName} has no {nameof(BillingSenderAdapterAttribute)} registration.");

            var key = (registration.ProviderId, registration.ResolutionType);
            if (!registrations.TryAdd(key, sender))
                throw new InvalidOperationException($"More than one billing sender is registered for provider {key.ProviderId}/{key.ResolutionType}.");
        }

        _senders = registrations;
    }

    public IBillingSender Resolve(int providerId, ResolutionType typeResolution) =>
        _senders.TryGetValue((providerId, typeResolution), out var sender)
            ? sender
            : throw new InvalidOperationException($"No sender found for provider {providerId}, {typeResolution}");

    private static Type GetUnderlyingSenderType(IBillingSender sender)
    {
        var current = sender.GetType();
        object currentSender = sender;
        var visited = new HashSet<Type>();
        while (visited.Add(current))
        {
            if (current.GetCustomAttributes(typeof(BillingSenderAdapterAttribute), inherit: false).Length > 0)
                return current;

            var wrapped = current
                .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(field => typeof(IBillingSender).IsAssignableFrom(field.FieldType))
                .Select(field => field.GetValue(currentSender) as IBillingSender)
                .FirstOrDefault(value => value is not null);

            wrapped ??= current
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(property => property.GetIndexParameters().Length == 0
                    && typeof(IBillingSender).IsAssignableFrom(property.PropertyType))
                .Select(property => property.GetValue(currentSender) as IBillingSender)
                .FirstOrDefault(value => value is not null);

            if (wrapped is null)
                return current;

            currentSender = wrapped;
            current = currentSender.GetType();
        }

        return current;
    }
}
