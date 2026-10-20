using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.CompanyProvider.DomainService.Impl;

public class CompanyProviderUpdateService(DataBaseContext context) :
    ICompanyProviderUpdateService
{
    public async Task<CompanyProviderEntity?> UpdateCompanyProviderAsync(CompanyProviderEntity entity, CancellationToken cancellationToken)
    {
        var existing = await context.CompanyProvider
            .FirstOrDefaultAsync(c => c.CompanyProviderId == entity.CompanyProviderId, cancellationToken);

        if (existing is null)
            return null;

        context.Entry(existing).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(cancellationToken);

        return existing;
    }
}