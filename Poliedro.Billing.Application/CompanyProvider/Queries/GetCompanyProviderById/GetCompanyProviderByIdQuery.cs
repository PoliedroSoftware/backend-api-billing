using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
namespace Poliedro.Billing.Application.CompanyProvider.Queries.GetCompanyProviderById;

public record GetCompanyProviderByIdQuery(Guid Id) : IRequest<CompanyProviderDto>;