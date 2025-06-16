using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class CustomSubtotalEntity
{

    [JsonPropertyName("concept")]
    public string? concept { get; set; }

    [JsonPropertyName("amount")]
    public double amount { get; set; }

}
