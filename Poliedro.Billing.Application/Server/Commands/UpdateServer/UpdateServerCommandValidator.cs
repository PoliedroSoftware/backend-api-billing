using FluentValidation;

namespace Poliedro.Billing.Application.Server.Commands.UpdateServer
{
    public class UpdateServerCommandValidator : AbstractValidator<UpdateServerCommand>
    {
        public UpdateServerCommandValidator()
        {
            RuleFor(x => x.ServerId).NotNull().GreaterThan(0)
                 .GreaterThan(0).WithMessage("The ServerId must be greater than 0.");

            RuleFor(x => x.Ip)
                .NotNull().WithMessage("The Ip cannot be null.")
                .NotEmpty().WithMessage("The Ip cannot be empty.");

            RuleFor(x => x.DatabaseName)
                .NotNull().WithMessage("The DatabaseName cannot be null.")
                .NotEmpty().WithMessage("The DatabaseName cannot be empty.");

            RuleFor(x => x.DbUsername)
                .NotNull().WithMessage("The DbUsername cannot be null.")
                .NotEmpty().WithMessage("The DbUsername cannot be empty.");

            RuleFor(x => x.DbPassword)
                .NotNull().WithMessage("The DbPassword cannot be null.")
                .NotEmpty().WithMessage("The DbPassword cannot be empty.");          
        }
    }
}
