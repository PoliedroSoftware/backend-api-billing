using FluentValidation;

namespace Poliedro.Billing.Application.Client.Commands.CreateClient
{
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("The name cannot be null.")
                .NotEmpty().WithMessage("The name cannot be empty.");

            RuleFor(x => x.Active)
                .Must(value => value == true || value == false).WithMessage("The active field must be a boolean value.");
        }
    }
}
