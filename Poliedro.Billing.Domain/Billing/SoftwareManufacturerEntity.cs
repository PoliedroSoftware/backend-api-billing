using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.BillingPos;

public class SoftwareManufacturerEntity
{
    [JsonPropertyName("ownerName")]
    public string ownerName { get; set; }
    [JsonPropertyName("softwareName")]
    public string softwareName { get; set; }
    [JsonPropertyName("companyName")]
    public string companyName { get; set; }
}
