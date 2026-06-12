
using Poliedro.Billing.Domain.CompanyProvider.Enums;

namespace Poliedro.Billing.Domain.CompanyProvider.Entities;

public class CompanyProviderEntity
{
    public Guid CompanyProviderId { get; set; }
    public int CompanyId { get; set; }
    public int ProviderId { get; set; }
    public int ServiceId { get; set; }
    public string? ApiUser { get; set; }
    public string? ApiPassword { get; set; }
    public string? ApiKey { get; set; }
    public EnvironmentType EnvironmentType { get; set; }
    public string? HeadNote { get; set; }
    public string? FooterNote { get; set; }

    public int Active { get; set; }

}
