using FluentValidation;
using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command;
using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail;
//using Poliedro.Billing.Application.Controllers.v1.FERetail;

using Poliedro.Billing.Domain.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail
{
    public class CreateFERetailCommandValidator : AbstractValidator<CreateFERetailCommand>
    {
        public CreateFERetailCommandValidator(IMessageProvider messageProvider)
        {
            //RuleFor(x => x.Date)
            //    .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
            //    .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            //RuleFor(x => x.Time)
            //    .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
            //    .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            //RuleFor(x => x.Prefix)
            //    .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
            //    .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            //RuleFor(x => x.Number)
            //    .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldGreatherThanZero);

            //RuleFor(x => x.OrderReference)
            //    .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull);

            //RuleForEach(x => x.Items).SetValidator(new ItemValidator(messageProvider));
            //RuleForEach(x => x.GeneralAllowances).SetValidator(new GeneralAllowanceValidator(messageProvider));
            //RuleForEach(x => x.AllTaxTotals).SetValidator(new TaxTotalValidator(messageProvider));

            //RuleFor(x => x.TotalToPay)
            //    .GreaterThanOrEqualTo(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeNonNegative);

            //RuleFor(x => x.Customer)
            //    .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
            //    .SetValidator(new CustomerValidator(messageProvider));

            //RuleFor(x => x.resolution)
            //    .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
            //    .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            //RuleFor(x => x.resolutionText)
            //    .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
            //    .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);
        }
    }

    public class ItemValidator : AbstractValidator<ItemElectronicEntity>
    {
        public ItemValidator(IMessageProvider messageProvider)
        {
            //RuleForEach(x => x.allowance_charges).SetValidator(new AllowanceChargeValidator(messageProvider)); 
            
            RuleFor(x => x.unit_measure_id)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.line_extension_amount)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.free_of_charge_indicator)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldIsRequired);


            RuleFor(x => x.allowance_charges)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.description)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            RuleFor(x => x.price_amount)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.code)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldIsRequired);
           
            RuleFor(x => x.type_item_identification_id)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.base_quantity)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.invoiced_quantity)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);
        }
    }

    public class AllowanceChargeValidator : AbstractValidator<AllowanceCharge>
    {
        public AllowanceChargeValidator(IMessageProvider messageProvider)
        {
            RuleFor(x => x.AllowanceChargeReason)
            .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldIsRequired);

            RuleFor(x => x.MultiplierFactorNumeric)
                .InclusiveBetween(0, 1).WithMessage(messageProvider.ErrorValidatorFieldMustBePercentage);

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.BaseAmount)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);
        }
    }

    public class GeneralAllowanceValidator : AbstractValidator<GeneralAllowance>
    {
        public GeneralAllowanceValidator(IMessageProvider messageProvider)
        {
            RuleFor(x => x.AllowanceChargeReason)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            RuleFor(x => x.AllowancePercent)
                .InclusiveBetween(0, 100).WithMessage(messageProvider.ErrorValidatorFieldMustBePercentage);

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeNonNegative);

            RuleFor(x => x.BaseAmount)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);
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
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator(IMessageProvider messageProvider)
        {
            RuleFor(x => x.IdentificationNumber)
                .NotNull().WithMessage(messageProvider.ErrorValidatorFieldNotNull)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldNotEmpty);

            RuleFor(x => x.Dv)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldIsRequired);


            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldIsRequired);


            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldIsRequired);
               

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldIsRequired);
               

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorInvalidEmail);

            RuleFor(x => x.MerchantRegistration)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorInvalidEmail);

            RuleFor(x => x.TypeDocumentIdentificationId)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.TypeOrganizationId)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.TypeLiabilityId)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.MunicipalityId)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);

            RuleFor(x => x.MunicipalityCode)
                .NotEmpty().WithMessage(messageProvider.ErrorValidatorFieldIsRequired);
 
            RuleFor(x => x.TypeRegimeId)
                .GreaterThan(0).WithMessage(messageProvider.ErrorValidatorFieldMustBeGreaterThanZero);
        }
    }
}
