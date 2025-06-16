using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;

public class ResponseData
{
    [JsonPropertyName("IsValid")]
    public string IsValid { get; set; }

    [JsonPropertyName("StatusDescription")]
    public string StatusDescription { get; set; }

    [JsonPropertyName("ErrorMessage")]
    public ErrorMessage ErrorMessage { get; set; }

    [JsonPropertyName("IssueDate")]
    public string IssueDate { get; set; }

    [JsonPropertyName("IssueTime")]
    public string IssueTime { get; set; }

    [JsonPropertyName("Filename")]
    public string Filename { get; set; }
    
}

