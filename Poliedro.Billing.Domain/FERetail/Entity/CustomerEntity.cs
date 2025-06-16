using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity
{
    public class CustomerEntity
    {
        [JsonPropertyName("identification_number")]
        public required string identification_number { get; set; }

        [JsonPropertyName("dv")]
        public string? dv { get; set; }

        [JsonPropertyName("name")]
        public required string name { get; set; }

        [JsonPropertyName("phone")]
        public string? phone { get; set; }

        [JsonPropertyName("address")]
        public required string address { get; set; }

        [JsonPropertyName("email")]
        public required string email { get; set; }

        [JsonPropertyName("merchant_registration")]
        public string? merchant_registration { get; set; }

        [JsonPropertyName("type_document_identification_id")]
        public required int type_document_identification_id { get; set; }

        [JsonPropertyName("type_organization_id")]
        public int? type_organization_id { get; set; }

        [JsonPropertyName("type_liability_id")]
        public required int type_liability_id { get; set; }

        [JsonPropertyName("municipality_id")]
        public int? municipality_id { get; set; }

        [JsonPropertyName("type_regime_id")]
        public required int type_regime_id { get; set; }
    }
}
