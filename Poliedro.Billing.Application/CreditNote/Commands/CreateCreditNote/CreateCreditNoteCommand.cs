using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CreditNote.Entity;

namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;

public record CreateCreditNoteCommand(
    string ApiKey,
    string Prefix,
    int Number,
    bool SendEmail,
    string Resolution,
    string ResolutionText,
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
) : IRequest<Result<ApiResponseCreditNote, Error>>;
