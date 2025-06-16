using FluentValidation;
using Poliedro.Billing.Domain.Ports;


namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote
{
    public class CreateCreditNoteCommandValidator : AbstractValidator<CreateCreditNoteCommand>
    {
        public CreateCreditNoteCommandValidator(IMessageProvider messageProvider)
        {
            RuleFor(x => x.Discrepancy)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull);

            RuleFor(x => x.Resolution)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            RuleFor(x => x.Prefix)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            RuleFor(x => x.Number)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);
            RuleFor(x => x.Number)
    .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

        }
    }

    public class ItemValidator : AbstractValidator<ItemElectronicEntity>
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
