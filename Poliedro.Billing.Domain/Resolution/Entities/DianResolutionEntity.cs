
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Domain.Resolution.Entities;
public class DianResolutionEntity
{
    public int ResolutionId { get; set; }
    public Guid CompanyProviderId { get; set; }
    public ResolutionType ResolutionType { get; set; }
    public string ResolutionNumber { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public int MultipleResolution { get; set; }
    public int VigencyMonth { get; set; }
    public int Automatic { get; set; }
    public int InitialRange { get; set; }
    public int FinalRange { get; set; }
    public int CurrentRange { get; set; }
    public DateTime ResolutionDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int ExpirationDays { get; set; }
    public int ExpirationNumber { get; set; }
    public bool Active { get; set; }

}
