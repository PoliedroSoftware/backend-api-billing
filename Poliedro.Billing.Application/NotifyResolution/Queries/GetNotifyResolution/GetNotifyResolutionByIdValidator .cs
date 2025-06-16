using FluentValidation;
namespace Poliedro.Billing.Application.NotifyResolution.Queries.GetNotifyResolution
{
    public class GetNotifyResolutionByIdValidator : AbstractValidator<GetNotifyResolutionByIdQuery>
    {
        public GetNotifyResolutionByIdValidator()
        {
            RuleFor(x => x.Parameters.ApiKey)
                .NotNull()
                .WithMessage("ID required");
        }
    }
}