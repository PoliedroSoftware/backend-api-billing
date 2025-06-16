﻿using FluentValidation;

namespace Poliedro.Billing.Application.Client.Commands.CreateClient
{
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("The name cannot be null.")
                .NotEmpty().WithMessage("The name cannot be empty.");

            RuleFor(x => x.ResolutionId)
                .NotNull().WithMessage("The resolution ID cannot be null.")
                .GreaterThan(0).WithMessage("The resolution ID must be greater than 0.");

            RuleFor(x => x.ServerId)
                .NotNull().WithMessage("The server ID cannot be null.")
                .GreaterThan(0).WithMessage("The server ID must be greater than 0.");

            RuleFor(x => x.ProviderId)
                .NotNull().WithMessage("The provider ID cannot be null.")
                .NotEmpty().WithMessage("The provider ID cannot be empty.");

            RuleFor(x => x.Active)
                .Must(value => value == true || value == false).WithMessage("The active field must be a boolean value.");
        }
    }
}
