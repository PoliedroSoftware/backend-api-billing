
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Domain.Resolution.Entities;
public class DianResolutionEntity
{
    public int ResolutionId { get; set; }
    public int CompanyProviderId { get; set; }
    public ResolutionType Document_type { get; set; }
    public string ResolutionNumber { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;


}
