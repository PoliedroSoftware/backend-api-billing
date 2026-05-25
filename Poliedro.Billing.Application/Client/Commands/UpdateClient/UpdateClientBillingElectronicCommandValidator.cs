using FluentValidation;

namespace Poliedro.Billing.Application.Client.Commands.UpdateClient
{
    public class UpdateClientBillingElectronicCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientBillingElectronicCommandValidator()
        {
            RuleFor(x => x.CompanyId).NotNull().GreaterThan(0)
                .GreaterThan(0).WithMessage("The ClientBillingElectronicId must be greater than 0.");
            RuleFor(x => x.Name)
                .NotNull().WithMessage("The name cannot be null.")
                .NotEmpty().WithMessage("The name cannot be empty.");

            RuleFor(x => x.Active)
                .Must(value => value == true || value == false).WithMessage("The active field must be a boolean value.");
        }
    }
}