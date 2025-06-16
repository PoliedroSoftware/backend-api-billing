using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity
{
    public class SoftwareManufacturerEntity
    {
        [JsonPropertyName("ownerName")]
        public required string ownerName { get; set; }
        [JsonPropertyName("softwareName")]
        public required string softwareName { get; set; }
        [JsonPropertyName("companyName")]
        public required string companyName { get; set; }
    }
}
