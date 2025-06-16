using FluentValidation;
using Poliedro.Billing.Application.Server.Commands.UpdateServer;
using Poliedro.Billing.Domain.Ports;
using System.Net;

namespace Poliedro.Billing.Application.Server.Commands.CreateServer;

public class CreateServerCommandValidator : AbstractValidator<CreateServerCommand>
{
    public CreateServerCommandValidator()
    {
        RuleFor(x => x.Ip)
            .NotNull().WithMessage("The IP address cannot be null.")
            .NotEmpty().WithMessage("The IP address is required.");

        RuleFor(x => x.DatabaseName)
            .NotNull().WithMessage("The database name cannot be null.")
            .NotEmpty().WithMessage("The database name is required.");

        RuleFor(x => x.DbUsername)
            .NotNull().WithMessage("The database username cannot be null.")
            .NotEmpty().WithMessage("The database username is required.");

        RuleFor(x => x.DbPassword)
            .NotNull().WithMessage("The database password cannot be null.")
            .NotEmpty().WithMessage("The database password is required.");
    }

    public class GetServerByIdCommandValidator : AbstractValidator<GetServerByIdCommand>
    {
        public GetServerByIdCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The ID must be greater than zero.");
        }
    }


}
