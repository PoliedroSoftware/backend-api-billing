using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;

/// <summary>
/// Compatibility wrapper for existing registrations. The provider-neutral registry
/// lives in Application so other infrastructure adapters can use it without
/// depending on the Plemsi project.
/// </summary>
public sealed class BillingSenderFactory(IEnumerable<IBillingSender> senders) : BillingSenderRegistry(senders)
{
}
