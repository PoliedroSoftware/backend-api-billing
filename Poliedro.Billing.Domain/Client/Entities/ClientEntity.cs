using Poliedro.Billing.Domain.Client.Enums;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.Client.Entities;

public class ClientEntity
{
    public int ClientBillingElectronicId { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public int ResolutionId { get; set; } = default!;
    public int ServerId { get; set; } = default!;
    public ProviderType ProviderId { get; set; } = default!;
    public bool Active { get; set; } = default!;
    public DianResolutionEntity DianResolution { get; set; } = default!;
    public ServerEntity Server { get; set; } = default!;
    public int Iterations { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
    public string ApiKey { get; set; } = string.Empty;
    public int Automatic { get; set; } = default!;
    public int MultipleResolution { get; set; } = default!;
    public string Email { get; set; } = string.Empty;
}
