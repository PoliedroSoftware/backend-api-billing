using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;

/// <summary>
/// Declares which provider and document type an infrastructure sender supports.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class BillingSenderAdapterAttribute(int providerId, ResolutionType resolutionType) : Attribute
{
    public int ProviderId { get; } = providerId;
    public ResolutionType ResolutionType { get; } = resolutionType;
}
