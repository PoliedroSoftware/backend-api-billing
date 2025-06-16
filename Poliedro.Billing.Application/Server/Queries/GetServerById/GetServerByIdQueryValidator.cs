using FluentValidation;

namespace Poliedro.Billing.Application.Server.Queries.GetServerById
{
    public class GetServerByIdQueryValidator : AbstractValidator<GetServerByIdQuery>
    {
        public GetServerByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("The Id cannot be null.")
                .GreaterThan(0).WithMessage("The Id must be greater than 0.");
        }
    }
}