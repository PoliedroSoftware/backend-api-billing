
namespace Poliedro.Billing.Domain.Client.Entities;

public class ClientEntity
{
    public int CompanyId { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Nit { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Active { get; set; } = default!;
}
