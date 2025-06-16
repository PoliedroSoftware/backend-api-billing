using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.Resolution.Entities;
public class DianResolutionEntity
{
    public int ResolutionId { get; set; }
    public string ResolutionNumber { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public int InitialRange { get; set; }
    public int FinalRange { get; set; }
    public DateTime ResolutionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ResolutionFile { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreationDate { get; set; }
    public int VigencyMonth { get; set; }
    public DateTime ExpirationDate { get; set; }
    public ResolutionType ResolutionType { get; set; }
    public int ClientBillingElectronicId { get; set; }
    public ICollection<ClientEntity> clientsBillingElectronic { get; set; }
    public int ExpirationDay { get; set; }
    public int ExpirationNumber { get; set; }
    public int CurrentlyNumber { get; set; }
    public string? CurrentlyDate { get; set; }
}
