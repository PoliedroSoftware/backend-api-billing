using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class CustomerEntity
{
    [JsonPropertyName("identification_number")]
    public  string? IdentificationNumber { get; set; }

    [JsonPropertyName("multiple_resolution")]
    public string? MultipleResolution { get; set; }

    [JsonPropertyName("api_key")]
    public string? ApiKey { get; set; }

    [JsonPropertyName("dv")]
    public string? Dv { get; set; }

    [JsonPropertyName("profit")]
    public int? Profit { get; set; }

    [JsonPropertyName("name")]
    public  string? Name { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("address")]
    public  string? Address { get; set; }

    [JsonPropertyName("email")]
    public  string? Email { get; set; }

    [JsonPropertyName("merchant_registration")]
    public string? MerchantRegistration { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }
   
    [JsonPropertyName("state")]
    public  string? State { get; set; }

    [JsonPropertyName("country")]
    public  string? Country { get; set; }    

    [JsonPropertyName("type_document_identification_id")]
    public int? TypeDocumentIdentificationId { get; set; }

    [JsonPropertyName("type_organization_id")]
    public int? TypeOrganizationId { get; set; }

    [JsonPropertyName("type_liability_id")]
    public int? TypeLiabilityId { get; set; }

    [JsonPropertyName("municipality_id")]
    public int? MunicipalityId { get; set; }

    [JsonPropertyName("municipality_code")]
    public string? MunicipalityCode { get; set; }

    [JsonPropertyName("type_regime_id")]
    public int? TypeRegimeId { get; set; }
}
