using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
namespace Poliedro.Billing.Application.CompanyProvider.Queries.GetCompanyProviderById;

public record GetCompanyProviderByIdQuery(int Id) : IRequest<CompanyProviderDto>;