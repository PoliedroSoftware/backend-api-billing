using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class AttachmentEntity
{
    [JsonPropertyName("filename")]
    public required string filename { get; set; } = string.Empty;

    [JsonPropertyName("b64data")]
    public required string b64data { get; set; } = string.Empty;
}
