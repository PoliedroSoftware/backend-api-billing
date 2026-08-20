using FluentValidation;
using Poliedro.Billing.Domain.CreditNote.Entity;
using Poliedro.Billing.Domain.Ports;


namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote
{
    public class CreateCreditNoteCommandValidator : AbstractValidator<CreateCreditNoteCommand>
    {
        public CreateCreditNoteCommandValidator()
        {
        }
    }

    public class ItemValidator : AbstractValidator<CreditNoteItem>
    {
        public ItemValidator(IMessageProvider messageProvider)
        {
            RuleFor(x => x.UnitMeasureId)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.LineExtensionAmount)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.Description)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            RuleFor(x => x.PriceAmount)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.Code)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);
        }
    }

    public class TaxTotalValidator : AbstractValidator<TaxTotal>
    {
        public TaxTotalValidator(IMessageProvider messageProvider)
        {
            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeNonNegative);

            RuleFor(x => x.Percent)
                .InclusiveBetween(0, 100).WithMessage(messageProvider.ErrorValidatorFieldMustBePercentage);
        }
    }
}
