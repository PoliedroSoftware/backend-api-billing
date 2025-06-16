

namespace Poliedro.Billing.Application.CustomersId.Dtos
{
    public record TributaryDto(
    string Liability,
    string Regime,
    string MerchantRegistration,
    List<string> Liabilities
);
}
