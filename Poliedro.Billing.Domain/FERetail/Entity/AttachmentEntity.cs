using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class AttachmentEntity
{
    [JsonPropertyName("filename")]
    public required string FileName { get; set; } = string.Empty;

    [JsonPropertyName("b64data")]
    public required string B64Data { get; set; } = string.Empty;
}
