using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class OrderReferenceDto
{
    [JsonPropertyName("id_order")]
    public string IdOrder { get; set; }
}
