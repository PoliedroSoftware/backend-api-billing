using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.Billing;

public class PayPointInfoEntity
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
    [JsonPropertyName("address")]
    public string Address { get; set; }
    [JsonPropertyName("cashierName")]
    public string CashierName { get; set; }
    [JsonPropertyName("payPointType")]
    public string PayPointType { get; set; }
    [JsonPropertyName("saleCode")]
    public string SaleCode { get; set; }
}
