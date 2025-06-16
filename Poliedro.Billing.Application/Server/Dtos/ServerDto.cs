using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Application.Server.Dtos;

public class ServerDto
{
    public int ServerId { get; set; } = default!;
    public string Ip { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string DbUsername { get; set; } = default!;
    public string DbPassword { get; set; } = default!;
    public DateTime CreationDate { get; set; } = default!;
    public List<ClientEntity> clients { get; set; } = [];
}
