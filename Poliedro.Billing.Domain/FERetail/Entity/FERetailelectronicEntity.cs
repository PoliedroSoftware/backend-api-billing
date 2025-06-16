using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.FERetail.Entity;

public class FERetailelectronicEntity
{
    [JsonPropertyName("date")]
    public required string date { get; set; }


    [JsonPropertyName("time")]
    public required string time { get; set; }


    [JsonPropertyName("prefix")]
    public required string prefix { get; set; }


    [JsonPropertyName("number")]
    public int number { get; set; }

    [JsonPropertyName("orderReference")]
    public required OrderReferenceEntity orderReference { get; set; }

    [JsonPropertyName("send_email")]
    public required bool send_email { get; set; }

    [JsonPropertyName("attachment1")]
    public AttachmentEntity? attachment1 { get; set; }

    [JsonPropertyName("attachment2")]
    public AttachmentEntity? attachment2 { get; set; }

    [JsonPropertyName("customer")]
    public required CustomerEntity customer { get; set; }

    [JsonPropertyName("payment")]
    public required PaymentEntity payment { get; set; }

    [JsonPropertyName("generalAllowances")]
    public List<GeneralAllowanceEntity>? generalAllowances { get; set; }

    [JsonPropertyName("items")]
    public required List<ItemElectronicEntity> items { get; set; }

    [JsonPropertyName("resolution")]
    public required string resolution { get; set; }

    [JsonPropertyName("resolutionText")]
    public string? resolutionText { get; set; }

    [JsonPropertyName("head_note")]
    public string? head_note { get; set; }

    [JsonPropertyName("foot_note")]
    public string? foot_note { get; set; }

    [JsonPropertyName("notes")]
    public string? notes { get; set; }
    
    [JsonPropertyName("allowanceTotal")]
    public double allowanceTotal { get; set; }

    [JsonPropertyName("invoiceBaseTotal")]
    public required double invoiceBaseTotal { get; set; }

    [JsonPropertyName("invoiceTaxExclusiveTotal")]
    public required double invoiceTaxExclusiveTotal { get; set; }

    [JsonPropertyName("invoiceTaxInclusiveTotal")]
    public required double invoiceTaxInclusiveTotal { get; set; }

    [JsonPropertyName("totalToPay")]
    public required double totalToPay { get; set; }

    [JsonPropertyName("allTaxTotals")]
    public List<AllTaxTotalEntity>? allTaxTotals { get; set; }

    [JsonPropertyName("allHoldingsTaxTotals")]
    public List<AllHoldingsTaxTotalEntity>? allHoldingsTaxTotals { get; set; }

    [JsonPropertyName("customSubtotals")]
    public List<CustomSubtotalEntity>? customSubtotals { get; set; }

    [JsonPropertyName("finalTotalToPay")]
    public required double finalTotalToPay { get; set; }

}

