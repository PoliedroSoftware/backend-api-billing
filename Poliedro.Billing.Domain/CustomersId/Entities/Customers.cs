

using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.CustomersId.Entities;

public class Customers
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = string.Empty;
    public Dni Dni { get; set; } = new();
    public Tributary Tributary { get; set; } = new();
    public string Company { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> AdditionalEmails { get; set; } = new();
    public bool IsActive { get; set; }

    [JsonPropertyName("__v")]
    public int __V { get; set; }
}
