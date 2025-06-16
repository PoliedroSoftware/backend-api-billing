namespace Poliedro.Billing.Application.SuccessInvoice.Dtos;

public record LegalMonetaryTotalsDto(
    string Line_Extension_Amount,
    string Tax_Exclusive_Amount,
    string Tax_Inclusive_Amount,
    string Payable_Amount
);