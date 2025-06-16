using System.Text.Json.Serialization;

namespace Poliedro.Billing.Domain.CustomersId.Entities
{
public class Tributary
{
        public string Liability { get; set; } = string.Empty;
        public string Regime { get; set; } = string.Empty;
        public string MerchantRegistration { get; set; } = string.Empty;
        public List<string> Liabilities { get; set; } = new();
    }
}

