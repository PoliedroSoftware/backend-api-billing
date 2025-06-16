using FluentValidation;

namespace Poliedro.Billing.Application.DianResolution.Queries.GetDianResolutionById
{
    public class GetDianResolutionByIdQueryValidator : AbstractValidator<GetDianResolutionByIdQuery>
    {
        public GetDianResolutionByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("The Id cannot be null.")
                .GreaterThan(0).WithMessage("The Id must be greater than 0.");
        }
    }
}