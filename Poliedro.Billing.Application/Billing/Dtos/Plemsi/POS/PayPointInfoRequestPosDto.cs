namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.POS;
public record PayPointInfoRequestPosDto
{
    public string Code { get; init; }
    public string Address { get; init; }
    public string CashierName { get; init; }
    public string PayPointType { get; init; }
    public string SaleCode { get; init; }
}
