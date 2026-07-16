using FluentValidation;

namespace Poliedro.Billing.Application.CompanyProvider.Commands.UpdateCompanyProvider;

public class UpdateCompanyProviderCommandValidator : AbstractValidator<UpdateCompanyProviderCommand>
{
    public UpdateCompanyProviderCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Data.CompanyId).GreaterThan(0);
        RuleFor(x => x.Data.ProviderId).GreaterThan(0);
        RuleFor(x => x.Data.ServiceId).GreaterThan(0);
        RuleFor(x => x.Data.EnvironmentType).IsInEnum();
    }
}