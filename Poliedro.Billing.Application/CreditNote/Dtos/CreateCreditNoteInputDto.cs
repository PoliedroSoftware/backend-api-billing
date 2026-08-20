using Poliedro.Billing.Domain.CreditNote.Entity;


namespace Poliedro.Billing.Application.CreditNote.Dtos;


public record CreateCreditNoteInputDto(
    bool SendEmail,
    string HeadNote,
    string FootNote,
    string Notes,
    decimal AllowanceTotal,
    decimal InvoiceBaseTotal,
    decimal InvoiceTaxExclusiveTotal,
    decimal InvoiceTaxInclusiveTotal,
    decimal TotalToPay,
    InvoiceReference InvoiceReference,
    Discrepancy Discrepancy,
    Customer Customer,
    Payment Payment,
    List<GeneralAllowance> GeneralAllowances,
    List<CreditNoteItem> Items,
    List<TaxTotal> AllTaxTotals,
    List<TaxTotal> AllHoldingsTaxTotals
);
