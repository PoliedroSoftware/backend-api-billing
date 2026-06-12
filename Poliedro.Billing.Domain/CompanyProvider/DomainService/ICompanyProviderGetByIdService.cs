
using Poliedro.Billing.Domain.CompanyProvider.Entities;

namespace Poliedro.Billing.Domain.CompanyProvider.DomainService;

public interface ICompanyProviderGetByIdService
{
    Task<CompanyProviderEntity> GetCompanyProviderByIdAsync(Guid Id, CancellationToken cancellationToken);
}
