namespace Poliedro.Billing.Application.SuccessInvoice.Dtos;

public record PaymentFormDto(
    List<int>? Payment_Form_Ids,
    List<int>? Payment_Method_Ids
);