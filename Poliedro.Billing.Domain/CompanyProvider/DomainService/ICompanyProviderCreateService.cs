using Poliedro.Billing.Domain.CompanyProvider.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Domain.CompanyProvider.DomainService;

public interface ICompanyProviderCreateService
{
    Task<CompanyProviderEntity> CreateCompanyProviderAsync(CompanyProviderEntity entity, CancellationToken cancellationToken);
}
