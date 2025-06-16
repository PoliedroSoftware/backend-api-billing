

namespace Poliedro.Billing.Domain.CustomersId.Entities
{
    public class Dni
    {
        public string Type { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string VerificationDigit { get; set; } = string.Empty;
    }
}
