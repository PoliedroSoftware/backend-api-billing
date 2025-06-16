using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.BillingPos;

public class PayPointInfoEntity
{
    [JsonPropertyName("code")]
    public string code { get; set; }
    [JsonPropertyName("address")]
    public string address { get; set; }
    [JsonPropertyName("cashierName")]
    public string cashierName { get; set; }
    [JsonPropertyName("payPointType")]
    public string payPointType { get; set; }
    [JsonPropertyName("saleCode")]
    public string saleCode { get; set; }
}
