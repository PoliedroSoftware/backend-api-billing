using Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;

namespace Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;

public record CreditNoteDTO
{

    public string prefix { get; init; }
    public int number { get; set; }
    public bool send_email { get; init; }
    public InvoiceReferenceDTO invoiceReference { get; init; }
    public DiscrepancyDTO discrepancy { get; init; }
    public required CustomerRequestFEDTO customer { get; init; }
    public required PaymentRequestFEDTO payment { get; init; }
    public List<GeneralAllowanceRequestFEDTO>? generalAllowances { get; init; }
    public required List<ItemElectronicRequestFEDTO> items { get; init; }
    public required string resolution { get; init; }
    public string? resolutionText { get; init; }
    public string? head_note { get; init; }
    public string? foot_note { get; init; }
    public string? notes { get; init; }
    public double allowanceTotal { get; init; }
    public double invoiceBaseTotal { get; init; }
    public double invoiceTaxExclusiveTotal { get; init; }
    public double invoiceTaxInclusiveTotal { get; init; }
    public double totalToPay { get; init; }
    public List<AllTaxTotalRequestFEDTO>? allTaxTotals { get; init; }
    public List<AllHoldingsTaxTotalRequestFEDTO>? allHoldingsTaxTotals { get; init;
    }
};
