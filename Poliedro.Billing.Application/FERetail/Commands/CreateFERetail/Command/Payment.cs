namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command
{
    public record Payment(
       int PaymentFormId,
       int PaymentMethodId,
       string PaymentDueDate,
       string DurationMeasure
    );
}

