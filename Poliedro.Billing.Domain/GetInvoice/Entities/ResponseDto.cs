using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;

public class ResponseDto
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public ResponseData Data { get; set; }
}

