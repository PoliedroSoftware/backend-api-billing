using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.CompanyProvider.DomainService.Impl;

public class CompanyProviderDeleteService(DataBaseContext context) :
    ICompanyProviderDeleteService
{
    public async Task<bool> DeleteCompanyProviderAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await context.CompanyProvider
            .FirstOrDefaultAsync(c => c.CompanyProviderId == id, cancellationToken);

        if (existing is null)
            return false;

        context.CompanyProvider.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}