using FluentValidation;

namespace Poliedro.Billing.Application.Client.Commands.DeleteClient;

public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
{
    public DeleteClientCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("The Id cannot be null.")
            .GreaterThan(0).WithMessage("The Id must be greater than 0.");
    }
}
