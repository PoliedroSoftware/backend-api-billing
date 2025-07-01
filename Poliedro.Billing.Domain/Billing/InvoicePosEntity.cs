using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.BillingPos;

public class InvoicePosEntity
{
    [JsonPropertyName("number")]
    public int number { get; set; }
    [JsonPropertyName("date")]
    public required string date { get; set; }
    [JsonPropertyName("time")]
    public required string time { get; set; }
    [JsonPropertyName("softwareManufacturer")]
    public SoftwareManufacturerEntity softwareManufacturer { get; set; }
    [JsonPropertyName("sendToEmail")]
    public required string sendToEmail { get; set; }
    [JsonPropertyName("resolution")]
    public required string resolution { get; set; }
    [JsonPropertyName("prefix")]
    public required string prefix { get; set; }
    [JsonPropertyName("head_note")]
    public string head_note { get; set; }
    [JsonPropertyName("foot_note")]
    public string foot_note { get; set; }
    [JsonPropertyName("payPointInfo")]
    public PayPointInfoEntity payPointInfo { get; set; }
    [JsonPropertyName("payment")]
    public PaymentPosEntity payment { get; set; }
    [JsonPropertyName("invoiceBaseTotal")]
    public required string invoiceBaseTotal { get; set; }
    [JsonPropertyName("invoiceTaxExclusiveTotal")]
    public required string invoiceTaxExclusiveTotal { get; set; }
    [JsonPropertyName("invoiceTaxInclusiveTotal")]
    public string invoiceTaxInclusiveTotal { get; set; }
    [JsonPropertyName("totalToPay")]
    public string totalToPay { get; set; }
    [JsonPropertyName("allTaxTotals")]
    public List<TaxTotalPosEntity> allTaxTotals { get; set; }
    [JsonPropertyName("items")]
    public List<ItemFERetailEntity> items { get; set; }
}
