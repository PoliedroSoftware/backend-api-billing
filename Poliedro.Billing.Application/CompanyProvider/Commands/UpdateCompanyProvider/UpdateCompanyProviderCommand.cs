using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Dtos;

namespace Poliedro.Billing.Application.CompanyProvider.Commands.UpdateCompanyProvider;

public record UpdateCompanyProviderCommand(int Id, UpdateCompanyProviderDto Data) : IRequest<CompanyProviderDto?>;