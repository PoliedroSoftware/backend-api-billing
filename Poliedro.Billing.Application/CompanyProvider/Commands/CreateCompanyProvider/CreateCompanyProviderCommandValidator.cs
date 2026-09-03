using FluentValidation;

namespace Poliedro.Billing.Application.CompanyProvider.Commands.CreateCompanyProvider;

public class CreateCompanyProviderCommandValidator : AbstractValidator<CreateCompanyProviderCommand>
{
    public CreateCompanyProviderCommandValidator()
    {
        RuleFor(x => x.Data.CompanyId).GreaterThan(0);
        RuleFor(x => x.Data.ProviderId).GreaterThan(0);
        RuleFor(x => x.Data.ServiceId).GreaterThan(0);
        RuleFor(x => x.Data.EnvironmentType).IsInEnum();
    }
}