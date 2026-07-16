using Poliedro.Billing.Domain.CompanyProvider.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Domain.CompanyProvider.DomainService;

public interface ICompanyProviderUpdateService
{
    Task<CompanyProviderEntity?> UpdateCompanyProviderAsync(CompanyProviderEntity entity, CancellationToken cancellationToken);
}
