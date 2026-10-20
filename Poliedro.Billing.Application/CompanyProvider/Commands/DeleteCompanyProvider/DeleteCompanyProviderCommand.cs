using MediatR;

namespace Poliedro.Billing.Application.CompanyProvider.Commands.DeleteCompanyProvider;

public record DeleteCompanyProviderCommand(int Id) : IRequest<bool>;