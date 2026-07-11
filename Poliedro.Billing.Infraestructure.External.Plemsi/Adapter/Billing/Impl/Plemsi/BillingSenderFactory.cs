using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;
public class BillingSenderFactory : IBillingSenderFactory
{
    private readonly IDictionary<(string, string), IBillingSender> _senders;
    public BillingSenderFactory(IEnumerable<IBillingSender> senders)
    {
            // Build dictionary by inspecting the actual underlying sender type.
            // Support decorated senders: if a decorator wraps the real sender, unwrap it via reflection.
            var dict = new Dictionary<(string, string), IBillingSender>();

            foreach (var s in senders)
            {
                var actualType = GetUnderlyingSenderType(s);

                if (actualType == typeof(BillingSenderFE))
                {
                    dict[("PLEMSI", "FE")] = s;
                }
                else if (actualType == typeof(BillingSenderPOS))
                {
                    dict[("PLEMSI", "POS")] = s;
                }
                else
                {
                    // If unknown, skip or throw depending on desired strictness
                    throw new Exception($"Unknown sender type: {actualType?.FullName ?? "null"}");
                }
            }

            _senders = dict;
        }

        private static Type? GetUnderlyingSenderType(IBillingSender sender)
        {
            var type = sender.GetType();

            // If the sender is the concrete type, return it
            if (type == typeof(BillingSenderFE) || type == typeof(BillingSenderPOS))
                return type;

            // Try to find a private field named '_innerSender' (our decorator pattern)
            var field = type.GetField("_innerSender", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                var inner = field.GetValue(sender) as IBillingSender;
                if (inner != null)
                    return inner.GetType();
            }

            // Try property 'Inner' or 'InnerSender' as fallback
            var prop = type.GetProperty("Inner", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                       ?? type.GetProperty("InnerSender", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prop != null)
            {
                var inner = prop.GetValue(sender) as IBillingSender;
                if (inner != null)
                    return inner.GetType();
            }

            // If still not found, return the runtime type
            return type;
        }


    public IBillingSender Resolve(string provider, ResolutionType typeResolution)
    {
        if (_senders.TryGetValue((provider, typeResolution.ToString()), out var sender))
            return sender;

        throw new InvalidOperationException($"No sender found for {provider}, {typeResolution}");
    }
}
