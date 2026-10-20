using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.CompanyProvider.DomainService.Impl;

public class CompanyProviderCreateService(DataBaseContext context) :
    ICompanyProviderCreateService
{
    public async Task<CompanyProviderEntity> CreateCompanyProviderAsync(CompanyProviderEntity entity, CancellationToken cancellationToken)
    {
        context.CompanyProvider.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
