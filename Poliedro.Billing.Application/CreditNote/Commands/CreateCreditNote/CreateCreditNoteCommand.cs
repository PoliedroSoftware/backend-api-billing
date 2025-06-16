using MediatR;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CreditNote.Entity;

namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;

public record CreateCreditNoteCommand(
    Discrepancy Discrepancy,
    string Resolution,
    string Prefix,
    int Number,
    List<ItemElectronicEntity> Items,
    Seller Seller,
    string FootNote,
    string HeadNote,
    InvoiceReference InvoiceReference,
    List<GeneralAllowance> GeneralAllowances,
    decimal AllowanceTotal,
    decimal InvoiceBaseTotal,
    decimal InvoiceTaxExclusiveTotal,
    decimal InvoiceTaxInclusiveTotal,
    decimal TotalToPay,
    List<TaxTotal> AllTaxTotals,
    List<TaxTotal> AllHoldingsTaxTotals
) : IRequest<Result<ApiResponseCreditNote, Error>>;
