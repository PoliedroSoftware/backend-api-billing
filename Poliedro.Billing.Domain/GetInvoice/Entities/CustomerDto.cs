using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class CustomerDto
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }

    [JsonPropertyName("company")]
    public string company { get; set; }

    [JsonPropertyName("invoice")]
    public string invoice { get; set; }

    [JsonPropertyName("identification_number")]
    public string IdentificationNumber { get; set; }

    [JsonPropertyName("dv")]
    public string dv { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("phone")]
    public string phone { get; set; }

    [JsonPropertyName("address")]
    public string address { get; set; }

    [JsonPropertyName("email")]
    public string email { get; set; }

    [JsonPropertyName("merchant_registration")]
    public string merchant_registration { get; set; }

    [JsonPropertyName("type_document_identification_id")]
    public int type_document_identification_id { get; set; }

    [JsonPropertyName("type_organization_id")]
    public int type_organization_id { get; set; }

    [JsonPropertyName("type_liability_id")]
    public int type_liability_id { get; set; }

    [JsonPropertyName("municipality_id")]
    public int municipality_id { get; set; }

    [JsonPropertyName("type_regime_id")]
    public int type_regime_id { get; set; }


}
