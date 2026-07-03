using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.CompanyProvider.DomainService.Impl;

public class CompanyProviderGetByIdSevice(DataBaseContext context) :
    ICompanyProviderGetByIdService
{
    public async Task<CompanyProviderEntity> GetCompanyProviderByIdAsync(int Id, CancellationToken cancellationToken)
    {
        return await context.CompanyProvider
            .FirstOrDefaultAsync(c => c.CompanyProviderId == Id, cancellationToken);
    }
}
