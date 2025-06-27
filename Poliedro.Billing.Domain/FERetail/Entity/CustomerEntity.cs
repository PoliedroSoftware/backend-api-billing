using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class CustomerEntity
{
    [JsonPropertyName("identification_number")]
    public required string IdentificationNumber { get; set; }

    [JsonPropertyName("dv")]
    public string? Dv { get; set; }

    public int? Profit { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("address")]
    public required string Address { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("merchant_registration")]
    public string? MerchantRegistration { get; set; }

    [JsonPropertyName("type_document_identification_id")]
    public required int TypeDocumentIdentificationId { get; set; }

    [JsonPropertyName("type_organization_id")]
    public int? TypeOrganizationId { get; set; }

    [JsonPropertyName("type_liability_id")]
    public required int TypeLiabilityId { get; set; }

    [JsonPropertyName("municipality_id")]
    public int? MunicipalityId { get; set; }

    [JsonPropertyName("type_regime_id")]
    public required int TypeRegimeId { get; set; }
}
