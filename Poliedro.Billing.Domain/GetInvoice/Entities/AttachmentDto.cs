using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;

public class AttachmentDto
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }

    [JsonPropertyName("company")]
    public string Company { get; set; }

    [JsonPropertyName("invoice")]
    public string Invoice { get; set; }

    [JsonPropertyName("filename")]
    public string Filename { get; set; }

    [JsonPropertyName("b64data")]
    public string B64data { get; set; }
}
