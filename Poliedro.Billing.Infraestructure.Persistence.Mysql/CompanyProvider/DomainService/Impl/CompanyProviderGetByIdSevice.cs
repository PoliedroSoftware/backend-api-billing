using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.CompanyProvider.DomainService.Impl;

public class CompanyProviderGetByIdSevice(DataBaseContext context) :
    ICompanyProviderGetByIdService
{
    public async Task<CompanyProviderEntity> GetCompanyProviderByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        return await context.CompanyProvider
            .FirstAsync(c => c.CompanyProviderId == Id, cancellationToken);
    }
}
