namespace Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.POS;
public record PayPointInfoRequestPosDto
{
    public string code { get; init; }
    public string address { get; init; }
    public string cashierName { get; init; }
    public string payPointType { get; init; }
    public string saleCode { get; init; }
}
