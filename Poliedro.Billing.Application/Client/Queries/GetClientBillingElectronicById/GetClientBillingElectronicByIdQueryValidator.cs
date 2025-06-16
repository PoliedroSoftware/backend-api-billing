using FluentValidation;

namespace Poliedro.Billing.Application.Client.Queries.GetClientBillingElectronicById;

public class GetClientBillingElectronicByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
{
    public GetClientBillingElectronicByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("The Id cannot be null.")
            .GreaterThan(0).WithMessage("The Id must be greater than 0.");
    }
}
