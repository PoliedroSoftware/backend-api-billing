using Poliedro.Billing.Domain.Client.Enums;
using Poliedro.Billing.Domain.Resolution.Enums;
namespace Poliedro.Billing.Domain.Billing;

public class BillingInfoClient
{
    public required string ApiKey { get; set; }
    public ResolutionType TypeResolution {  get; set; }
    public ProviderType ProviderType {  get; set; }
    public required string Provider { get; set; }
    public required string Prefix {  get; set; }
    public required DateTime ExpirationDate {  get; set; }
    public required int FinalRange {  get; set; }
    public required string ResolucionNumber { get; set; }
    public required string Descripcion { get; set; }
    public required int CurrentlyNumber { get; set; }
    public required int MultipleResolution { get; set; }

    public required int ResolutionId { get; set; }
}
