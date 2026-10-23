namespace Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.POS;
public record SoftwareManufacturerRequestPosDto
{
    public string ownerName { get; init; }
    public string softwareName { get; init; }
    public string companyName { get; init; }
}
