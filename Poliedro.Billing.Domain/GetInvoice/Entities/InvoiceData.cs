using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.GetInvoice.Entities;

public class InvoiceData
{
    [JsonPropertyName("order_reference")]
    public OrderReferenceDto OrderReference { get; set; }

    [JsonPropertyName("legal_monetary_totals")]
    public LegalMonetaryTotalsDto LegalMonetaryTotals { get; set; }

    [JsonPropertyName("_id")]
    public string Id { get; set; }

    [JsonPropertyName("company")]
    public string Company { get; set; }

    [JsonPropertyName("createdBy")]
    public string CreatedBy { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("preview")]
    public string Preview { get; set; }

    [JsonPropertyName("resolution_number")]
    public string ResolutionNumber { get; set; }

    [JsonPropertyName("prefix")]
    public string Prefix { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; }

    [JsonPropertyName("consecutive")]
    public string Consecutive { get; set; }

    [JsonPropertyName("type_document_id")]
    public int TypeDocumentId { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("emailDeliveryStatus")]
    public string EmailDeliveryStatus { get; set; }

    [JsonPropertyName("response")]
    public ResponseDto Response { get; set; }

    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public string UpdatedAt { get; set; }

    [JsonPropertyName("customer")]
    public CustomerDto Customer { get; set; }

    [JsonPropertyName("payment_form")]
    public PaymentFormDto PaymentForm { get; set; }

    [JsonPropertyName("tax_totals")]
    public List<TaxTotalDto> TaxTotals { get; set; } = [];

    [JsonPropertyName("with_holding_tax_total")]
    public List<WithholdingTaxTotalDto> WithHoldingTaxTotal { get; set; } = [];

    [JsonPropertyName("head_note")]
    public string HeadNote { get; set; }

    [JsonPropertyName("foot_note")]
    public string FootNote { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; }

    [JsonPropertyName("allowance_charges")]
    public List<AllowanceChargeDto> AllowanceCharges { get; set; } = [];

    [JsonPropertyName("attachmentstechProviderDefaultFootNote")]
    public List<AttachmentDto> Attachments { get; set; } = [];

    [JsonPropertyName("TechProviderDefaultFootNote")]
    public string TechProviderDefaultFootNote { get; set; }
}
