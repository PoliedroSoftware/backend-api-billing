using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;

public class ApiResponse<T>
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("info")]
    public string Info { get; set; }

    [JsonPropertyName("data")]
    public T Data { get; set; }
}
