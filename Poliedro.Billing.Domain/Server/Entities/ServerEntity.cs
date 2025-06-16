using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.Server.Entities;

public class ServerEntity
{
    public int ServerId { get; set; } = default!;
    public string Ip { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string DbUsername { get; set; } = default!;
    public string DbPassword { get; set; } = default!;
    public uint Port { get; set; } = default!;

    public ICollection<ClientEntity> clientsBillingElectronic { get; set; } = default!;
}
