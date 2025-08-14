using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class CustomSubtotalEntity
{

    [JsonPropertyName("concept")]
    public string? Concept { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; }

}
