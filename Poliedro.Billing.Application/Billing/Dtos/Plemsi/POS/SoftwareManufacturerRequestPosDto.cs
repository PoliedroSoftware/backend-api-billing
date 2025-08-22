namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.POS;
public record SoftwareManufacturerRequestPosDto
{
    public string ownerName { get; init; }
    public string softwareName { get; init; }
    public string companyName { get; init; }
}
