using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Domain.CompanyProvider.DomainService;

public interface ICompanyProviderDeleteService
{
    Task<bool> DeleteCompanyProviderAsync(int id, CancellationToken cancellationToken);
}
